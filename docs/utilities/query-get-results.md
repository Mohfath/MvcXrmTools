<div dir="rtl">

<!-- nav-top -->
[« صفحه‌ی اصلی](../../README.md) › [ابزارهای عمومی](../../README.md#utilities)

# گرفتن نتیجه‌ی جستجو به‌صورت جدول، لیست یا مقدار در Dynamics CRM (استپ Query Get Results)

**استپ Query Get Results یک View یا یک FetchXml را اجرا می‌کند و نتیجه را در چند شکل آماده برمی‌گرداند:** جدول HTML رنگی برای ایمیل، متن CSV، لیست جداشده با ویرگول، تعداد نتایج و مقدار اولین سلول به‌صورت‌های مختلف (عدد، تاریخ، پول، شناسه و متن). برای ساختن ایمیل‌هایی که خلاصه‌ی رکوردها را دارند یا گرفتن یک مقدار از یک جستجو (مثل مجموع فروش) مناسب است.

برای استفاده از این استپ، در طراحی Workflow از منوی **Add Step** به گروه **MvcTeam Utilities** بروید و **Query Get Results** را انتخاب کنید.

## پارامترهای ورودی

### تعریف جستجو (فقط یکی کافی است)

* **Pick a System View to Use** : یک View سیستمی (Lookup به System View).
* **or pick a Personal View to Use** : یک View شخصی (Lookup به Personal View).
* **or enter FetchXML to Use** : متن FetchXml.

اولویت: اگر بیش از یکی پر شود، **View شخصی**، بعد **View سیستمی** و بعد **FetchXml** استفاده می‌شود. اگر هیچ‌کدام پر نباشد، استپ با خطای `You need to pick a System View, a Personal View, or specify FetchXML for the query.` متوقف می‌شود.

### ظاهر جدول HTML

همه‌ی رنگ‌ها باید به شکل **`#xxxxxx`** (کد Hex شش‌رقمی) باشند، مثلاً `#6495ED`؛ در غیر این صورت استپ با خطای `Hex code invalid for '…' - value must be formatted as '#xxxxxx'` متوقف می‌شود.

| پارامتر | پیش‌فرض | توضیح |
|---|---|---|
| **Table - Include Header** | `True` | ردیف عنوان ستون‌ها را در جدول و CSV بیاورد یا نه. |
| **Table - Border Color** | `#000000` | رنگ حاشیه‌ی جدول. |
| **Table - Header Background Color** | `#6495ED` | رنگ پس‌زمینه‌ی ردیف عنوان. |
| **Table - Header Font Color** | `#FFFFFF` | رنگ نوشته‌ی عنوان. |
| **Table - Header Bold Font?** | `True` | عنوان‌ها پررنگ باشند یا نه. |
| **Table - Row Background Color** | `#FFFFFF` | رنگ پس‌زمینه‌ی ردیف‌ها. |
| **Table - Row Font Color** | `#000000` | رنگ نوشته‌ی ردیف‌ها. |
| **Table - Alternating Row Background Color** | `#F0F8FF` | رنگ پس‌زمینه‌ی ردیف‌های زوج (یک‌درمیان). |
| **Table - Alternating Row Font Color** | `#000000` | رنگ نوشته‌ی ردیف‌های زوج. |

### ظاهر لیست

* **List - Item separator** (پیش‌فرض: `, `) : جداکننده‌ی اقلام لیست.
* **List - Include empty items?** (پیش‌فرض: `False`) : اگر `True` باشد، خانه‌های خالی هم در لیست بیایند.

## پارامترهای خروجی

| خروجی | توضیح |
|---|---|
| **# of Results** | تعداد ردیف‌های نتیجه. |
| **Table - Query Results (HTML)** | نتیجه به‌صورت جدول HTML رنگی (برای بدنه‌ی ایمیل). |
| **Table - Query Results (CSV)** | نتیجه به‌صورت متن CSV. |
| **List - Query Results as a List** | مقدار **ستون اول** هر ردیف، با جداکننده‌ی List - Item separator به هم وصل‌شده. |
| **Single Value - Result as Text** | مقدار **ستون اول ردیف اول** به‌صورت متن (همان‌طور که کاربر می‌بیند). |
| **Single Value - Result as Whole Number** | همان مقدار به‌صورت عدد صحیح (اگر قابل تبدیل باشد). |
| **Single Value - Result as Decimal** | همان مقدار به‌صورت عدد اعشاری (اگر قابل تبدیل باشد). |
| **Single Value - Result as Double** | همان مقدار به‌صورت Double (اگر قابل تبدیل باشد). |
| **Single Value - Result as Money** | همان مقدار به‌صورت پول (اگر عددی باشد). |
| **Single Value - Result as DateTime** | همان مقدار به‌صورت تاریخ (اگر تاریخ باشد). |
| **Single Value - Result as ID** | همان مقدار به‌صورت شناسه (GUID) (اگر شناسه باشد؛ مثلاً برای ستون Lookup). |

خروجی‌های «Single Value» فقط وقتی مقدار می‌گیرند که مقدار ستون اول ردیف اول به آن نوع قابل تبدیل باشد؛ وگرنه خالی می‌مانند. اگر جستجو هیچ ردیفی نداشته باشد، همه‌ی آن‌ها خالی می‌مانند و **# of Results** برابر `0` است.

## نحوه‌ی کار

* **ستون‌ها:** اگر از View استفاده کنید، ستون‌های View (بر اساس Layout آن) ستون‌های جدول هستند. اگر از FetchXml استفاده کنید، ستون‌ها فیلدهای (attribute) نوشته‌شده در FetchXml هستند.
* **عنوان ستون:** اول نام مستعار (alias) فیلد در FetchXml، بعد نام نمایشی فیلد به زبان کاربر و در نهایت نام منطقی.
* **مقدارها:** در جدول و CSV و لیست، هر سلول همان متنی است که کاربر در CRM می‌بیند؛ مثلاً برای Lookup نام رکورد و برای Option Set برچسب گزینه. برای خروجی‌های «Single Value» از مقدار ماشینی استفاده می‌شود: شناسه‌ی رکورد برای Lookup، شماره‌ی گزینه برای Option Set، تاریخ ISO به UTC و اعشار با نقطه.
* **چند صفحه:** همه‌ی صفحه‌های نتیجه خوانده می‌شود.
* **رکورد جاری:** اگر در FetchXml یا View، فیلتری «شناسه‌ی رکورد خالی نباشد» روی یک Link به موجودیت رکورد جاری Workflow باشد، نتیجه به همان رکورد جاری محدود می‌شود؛ روش معمول برای گرفتن «رکوردهای مرتبط با رکورد فعلی».
* **لیست‌های گیرنده (To و CC و …)** در ایمیل‌ها به‌صورت نام گیرنده‌ها نمایش داده می‌شوند.
* استپ با دسترسی کاربر اجراکننده‌ی Workflow اجرا می‌شود؛ پس فقط رکوردهایی را می‌بیند که آن کاربر اجازه‌ی خواندنشان را دارد.

## مثال کاربرد

* **جدول فاکتورهای عقب‌افتاده در ایمیل:** یک View «فاکتورهای عقب‌افتاده‌ی این مشتری» بسازید یا FetchXml بنویسید، خروجی HTML را در بدنه‌ی [Send Email](../email/send-email.md) یا یک ایمیل بگذارید.
* **مجموع فروش یک ماه:** یک FetchXml تجمیعی (sum) بنویسید و از **Single Value - Result as Money** یا **Decimal** برای گرفتن مجموع استفاده کنید.
* **فهرست نام همه‌ی مخاطبین یک شرکت:** از **List - Query Results as a List** استفاده کنید.

## نکته‌ها

* برای اجرای یک Workflow روی همه‌ی رکوردهای نتیجه از [Query Run Workflow On Results](query-run-workflow-on-results.md) و برای خواندن حداکثر دو فیلد از اولین رکورد مطابق دو شرط ساده از [Query Values](query-values.md) استفاده کنید.

---

منبع: این استپ بر پایه‌ی استپ QueryGetResults از پروژه‌ی متن‌باز [WorkflowElements](https://github.com/akaskela/WorkflowElements) (نوشته‌ی Aiden Kaskela، مجوز MIT) است.

---

<!-- nav-bottom -->
[⬆ بازگشت به فهرست اصلی و همه‌ی استپ‌ها](../../README.md#steps)

</div>
