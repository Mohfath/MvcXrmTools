<div dir="rtl">

<!-- nav-top -->
[« صفحه‌ی اصلی](../../README.md) › [ابزارهای عمومی](../../README.md#utilities)

# تغییر فیلد با نام فیلد داینامیک در Dynamics CRM (استپ Update Field Dynamically)

این استپ یک **فیلد مشخص** از یک رکورد را با مقدار جدید به‌روز می‌کند؛ در حالی که هم رکورد و هم نام فیلد را به‌صورت داینامیک (به‌شکل متن) به آن می‌دهید. برای مواقعی مفید است که نوع رکورد یا نام فیلد در زمان طراحی Workflow معلوم نیست.

برای استفاده از این استپ، در طراحی Workflow از منوی **Add Step** به گروه **MvcTeam Utilities** بروید و **Update Field Dynamically** را انتخاب کنید.

## پارامترهای ورودی

* **This Record DynamicUrl** : آدرس داینامیک رکوردی که به‌روز می‌شود (**Record URL (Dynamic)**). اگر خالی باشد استپ با خطای `This Record DynamicUrl is required.` متوقف می‌شود.
* **Field To Update** : نام منطقی (logical name) فیلدی که تغییر می‌کند.
* **int,decimal,money,string,lookup** : نوع فیلد. یکی از مقدارهای زیر (با حروف کوچک):

| مقدار | نوع فیلد | مقدار `New Value` |
|---|---|---|
| `string` | متن | هر متن |
| `int` | عدد صحیح | عدد صحیح |
| `decimal` | عدد اعشاری | عدد اعشاری |
| `money` | پول (Currency) | عدد اعشاری |
| `optionset` | Option Set | شماره‌ی گزینه (عدد صحیح) |
| `lookup` | Lookup | استفاده نمی‌شود؛ از دو پارامتر Lookup در ادامه استفاده کنید |

* **Lookup Target Entity Logical Name** : نام منطقی نوع رکوردی که Lookup به آن اشاره می‌کند (فقط برای نوع `lookup`).
* **New Value (Empty if using Lookup)** : مقدار جدید به‌صورت متن. برای نوع `lookup` خالی بگذارید.
* **Lookup Target Guid** : GUID رکوردی که Lookup به آن اشاره می‌کند (فقط برای نوع `lookup`).

این استپ پارامتر خروجی ندارد.

## نکته‌ی مهم: استپ فقط وقتی کار می‌کند که هم نام فیلد و هم نوع آن پر باشند

اگر **Field To Update** یا نوع فیلد خالی باشد، استپ **بدون خطا و بدون هیچ تغییری** رد می‌شود.

## خطاها

| پیام | علت |
|---|---|
| `This Record DynamicUrl is required.` | آدرس رکورد خالی است. |
| `New Value is required for field type '…'.` | برای نوعی غیر از `string` و `lookup` مقدار خالی داده شده است. |
| `New Value '…' is not a valid …` | مقدار با نوع فیلد هم‌خوان نیست (مثلاً متن به‌جای عدد). |
| `Unsupported field type '…'. Use one of: string, int, decimal, money, optionset, lookup.` | نوع فیلد ناشناخته یا با حروف بزرگ نوشته شده است. |
| `Lookup Target Guid '…' is not a valid GUID.` | GUID رکورد هدف معتبر نیست (برای `lookup`). |

## نکته‌ها

* استپ با دسترسی کاربر اجراکننده‌ی Workflow اجرا می‌شود و فقط **همان یک فیلد** را به‌روز می‌کند.
* نوع `string` مقدار خالی را هم می‌پذیرد (متن خالی را ذخیره می‌کند).
* نوع `lookup` نیاز به **هر دو** پارامتر Lookup Target Entity Logical Name و Lookup Target Guid دارد.

---

<!-- nav-bottom -->
[⬆ بازگشت به فهرست اصلی و همه‌ی استپ‌ها](../../README.md#steps)

</div>
