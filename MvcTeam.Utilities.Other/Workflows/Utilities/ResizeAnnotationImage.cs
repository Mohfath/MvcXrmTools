using System;
using System.Activities;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;

public class Utilities_ResizeAnnotationImage : CodeActivity
{
	[Input("Annotation")]
	[ReferenceTarget("annotation")]
	public InArgument<EntityReference> Annotation { get; set; }

	[Input("QualityPercent")]
	public InArgument<int> QuualityPercent { get; set; }

	protected override void Execute(CodeActivityContext context)
	{
		try
		{
			IWorkflowContext workflowContext = context.GetExtension<IWorkflowContext>();

			IOrganizationServiceFactory serviceFactory = context.GetExtension<IOrganizationServiceFactory>();

			// Use the context service to create an instance of IOrganizationService.             
			IOrganizationService _service = serviceFactory.CreateOrganizationService(workflowContext.InitiatingUserId);
			ITracingService tracingService = context.GetExtension<ITracingService>();
			EntityReference entityReference = Annotation.Get<EntityReference>(context);

				//An empty input arrives as 0, which would save every image at the worst quality and overwrite the original
				int quality = QuualityPercent.Get(context);
				if (quality == 0) quality = 90;
				if (quality < 1 || quality > 100)
					throw new InvalidPluginExecutionException("QualityPercent must be between 1 and 100.");
			
			QueryExpression queryExpression = new QueryExpression("annotation");
			ConditionExpression item = new ConditionExpression("annotationid", ConditionOperator.Equal, entityReference.Id);
			queryExpression.Criteria.Conditions.Add(item);
			queryExpression.ColumnSet = new ColumnSet("filename", "documentbody");
			EntityCollection entityCollection = _service.RetrieveMultiple(queryExpression);
			List<string> list = new List<string>();
			list.Add("jpg");
			list.Add("png");
			list.Add("jpeg");
			foreach (Entity entity in entityCollection.Entities)
			{
				if (!entity.Attributes.ContainsKey("filename"))
				{
					continue;
				}
				string text2 = entity.Attributes["filename"].ToString();
				if (text2.Contains("-optimise"))
				{
					continue;
				}
				string[] array = text2.Split('.');
				string text3 = array[array.Length - 1].ToLower();
				if (list.Contains(text3))
				{
					ImageFormat format = ImageFormat.Jpeg;
					if (text3 == "png")
					{
						format = ImageFormat.Png;
					}
					byte[] array2 = Convert.FromBase64String(entity["documentbody"].ToString());
					int num;
					int newHeight;
					using (MemoryStream stream = new MemoryStream(array2))
					using (Bitmap bitmap = new Bitmap(stream))
					{
						//تصاویر کوچکتر از 800 پیکسل بزرگ نمیشوند
						num = Math.Min(800, bitmap.Width);
						newHeight = num * bitmap.Height / bitmap.Width;
					}
					byte[] imageByte = GenerateThumbnails(num, newHeight, array2, format);
					byte[] inArray = CompressImageWithQuality(imageByte, format, quality);
					entity["documentbody"] = Convert.ToBase64String(inArray);
					string str = string.Join(".", array, 0, array.Length - 1);
					str += "-optimise";
					str = (string)(entity["filename"] = str + "." + array[array.Length - 1]);
					_service.Update(entity);
				}
				else
				{
					
				}
			}
		}
		catch (Exception innerException)
		{
			throw new InvalidPluginExecutionException("خطا در تغییر سایز",innerException);
		}
	}

	public static byte[] CompressImageWithQuality(byte[] imageByte, ImageFormat format, int jpegQuality = 90)
	{
		using (MemoryStream stream = new MemoryStream(imageByte))
		{
			ImageCodecInfo encoder = GetEncoder(format);
			using (EncoderParameters encoderParameters = new EncoderParameters(1))
			using (Image image = Image.FromStream(stream))
			using (MemoryStream memoryStream = new MemoryStream())
			{
				encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, jpegQuality);
				image.Save(memoryStream, encoder, encoderParameters);
				return memoryStream.ToArray();
			}
		}
	}

	private static ImageCodecInfo GetEncoder(ImageFormat format)
	{
		ImageCodecInfo[] imageDecoders = ImageCodecInfo.GetImageDecoders();
		ImageCodecInfo[] array = imageDecoders;
		foreach (ImageCodecInfo imageCodecInfo in array)
		{
			if (imageCodecInfo.FormatID == format.Guid)
			{
				return imageCodecInfo;
			}
		}
		return null;
	}

	private byte[] GenerateThumbnails(int newWidth, int newHeight, byte[] imageByte, ImageFormat format)
	{
		using (MemoryStream stream = new MemoryStream(imageByte))
		{
			using (Image image = Image.FromStream(stream))
			using (Bitmap bitmap = new Bitmap(newWidth, newHeight))
			using (Graphics graphics = Graphics.FromImage(bitmap))
			using (MemoryStream memoryStream = new MemoryStream())
			{
				graphics.CompositingQuality = CompositingQuality.HighQuality;
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
				Rectangle rect = new Rectangle(0, 0, newWidth, newHeight);
				graphics.DrawImage(image, rect);
				bitmap.Save(memoryStream, format);
				return memoryStream.ToArray();
			}
		}
	}
}
