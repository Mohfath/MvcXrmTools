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

public class ResizeAnnotationImage : CodeActivity
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
			
			QueryExpression queryExpression = new QueryExpression("annotation");
			ConditionExpression item = new ConditionExpression("annotationid", ConditionOperator.Equal, entityReference.Id);
			queryExpression.Criteria.Conditions.Add(item);
			queryExpression.ColumnSet = new ColumnSet(allColumns: true);
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
					MemoryStream stream = new MemoryStream(array2);
					Bitmap bitmap = new Bitmap(stream);
					int num = 800;
					int newHeight = num * bitmap.Height / bitmap.Width;
					byte[] imageByte = GenerateThumbnails(num, newHeight, array2);
					byte[] inArray = CompressImageWithQuality(imageByte, format, QuualityPercent.Get(context));
					entity["documentbody"] = Convert.ToBase64String(inArray);
					string str = array[0];
					for (int i = 1; i < array.Length - 1; i++)
					{
						str = string.Join(".", array);
					}
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
			EncoderParameters encoderParameters = new EncoderParameters(1);
			encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, jpegQuality);
			Image image = Image.FromStream(stream);
			MemoryStream memoryStream = new MemoryStream();
			image.Save(memoryStream, encoder, encoderParameters);
			return memoryStream.ToArray();
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

	private byte[] GenerateThumbnails(int newWidth, int newHeight, byte[] imageByte)
	{
		using (MemoryStream stream = new MemoryStream(imageByte))
		{
			Image image = Image.FromStream(stream);
			Bitmap bitmap = new Bitmap(newWidth, newHeight);
			Graphics graphics = Graphics.FromImage(bitmap);
			graphics.CompositingQuality = CompositingQuality.HighQuality;
			graphics.SmoothingMode = SmoothingMode.HighQuality;
			graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
			Rectangle rect = new Rectangle(0, 0, newWidth, newHeight);
			graphics.DrawImage(image, rect);
			MemoryStream memoryStream = new MemoryStream();
			bitmap.Save(memoryStream, ImageFormat.Jpeg);
			return memoryStream.ToArray();
		}
	}
}
