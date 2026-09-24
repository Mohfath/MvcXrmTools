<div dir="rtl">

# MvcTeam Utilities

## مجموعه‌ی 146 استپ سفارشی Workflow برای Microsoft Dynamics CRM (On-Premise)، با پشتیبانی کامل از فارسی

> **رایگان، متن‌باز (MIT) و ساخته‌شده برای کاربران ایرانی Dynamics CRM.** اگر Workflow می‌سازید و می‌خواهید **تاریخ شمسی (جلالی)**، **مبلغ به حروف فارسی**، **اعتبارسنجی کد ملی** و **روز کاری با آخر هفته‌ی ایران** را بدون نوشتن کد در **Microsoft Dynamics CRM** داشته باشید، جای درستی آمده‌اید.

**MvcTeam Utilities** مجموعه‌ای از Custom Workflow Activityها برای **Microsoft Dynamics CRM نسخه‌ی On-Premise** است. بعد از Import کردن Solution، همه‌ی 146 استپ در طراحی Workflow (بخش **Add Step**) در دسترس‌اند و بدون برنامه‌نویسی می‌توانید با آن‌ها متن، عدد، تاریخ، ایمیل، یادداشت و دسترسی‌ها را مدیریت کنید. هر استپ یک صفحه‌ی مستند فارسی با پارامترها، مثال‌های واقعی و نکته‌های مهم دارد.

### چرا این پروژه ساخته شد؟

این ابزارها را ساخته‌ایم تا کار روزمره‌ی شما ساده‌تر شود: کد کمتر، Workflow سریع‌تر و وقت بیشتر برای کار اصلی. باور داریم زیرساخت نرم‌افزاری قوی و مستقل، پایه‌ی سازمان‌ها و کسب‌وکارهای قوی است؛ پس آن را رایگان و آزاد در اختیار همه‌ی تیم‌های فنی و سازمان‌های ایرانی می‌گذاریم تا از آن استفاده کنند، یاد بگیرند و آن را بهتر کنند. این پروژه سهم کوچکی است برای زیرساختی محکم‌تر و با امید به **آینده‌ای آزاد برای ایران**.

## فهرست مطالب

* [معرفی](#intro)
* [سازگاری و نیازمندی‌ها](#requirements)
* [نصب و راه‌اندازی](#install)
* [استفاده در Workflow](#usage)
* [نکات مهم پیش از شروع](#notes)
* [فهرست همه‌ی استپ‌ها](#steps)
* [ساختار پوشه‌ها](#layout)
* [منابع و قدردانی](#credits)
* [مجوز](#license)

<a id="intro"></a>

## معرفی

Workflow در Dynamics CRM ابزار قدرتمندی برای خودکارسازی کارهاست، ولی استپ‌های پیش‌فرض آن برای کارهای روزمره کافی نیستند: نمی‌شود مبلغ را به حروف نوشت، تاریخ شمسی ساخت، متن را برش داد، رکورد را کلون کرد یا گیرنده‌های ایمیل را از یک تیم اضافه کرد. MvcTeam Utilities این خلأها را با **146 استپ آماده** پر می‌کند. هر استپ یک صفحه‌ی مستند فارسی دارد که پارامترها، نحوه‌ی کار، مثال‌های واقعی و نکته‌های مهم آن را توضیح می‌دهد.

**چه چیزی به شما می‌دهد؟**

* **ابزارهای فارسی:** مبلغ به حروف فارسی، تاریخ شمسی با قالب دلخواه و ارقام فارسی، سال و ماه و فصل شمسی، اعتبارسنجی کد ملی.
* **تاریخ و زمان (47 استپ):** جمع و تفریق، اختلاف، اجزای تاریخ، شروع و پایان ماه و فصل و سال، گرد کردن زمان و روز کاری با آخر هفته‌ی قابل تنظیم برای ایران.
* **متن و عدد (49 استپ):** جستجو، جایگزینی، Regular Expression، Base64، پُر کردن، حساب چهارعمل، گرد کردن، ریشه و توان.
* **ایمیل و یادداشت:** افزودن گیرنده‌ها از تیم و نقش و صف، ارسال ایمیل، مدیریت پیوست‌ها و یادداشت‌ها.
* **ابزارهای رکورد و دسترسی:** کلون رکورد، نتیجه‌ی جستجو به‌صورت جدول، اجرای Workflow روی نتایج، بررسی نقش و تیم، تغییر تنظیمات کاربر.

<a id="requirements"></a>

## سازگاری و نیازمندی‌ها

* **این استپ‌ها برای Microsoft Dynamics CRM On-Premise ساخته شده‌اند.** به‌صورت Custom Workflow Activity روی سرور CRM ثبت می‌شوند و نیازی به سرویس ابری یا افزونه‌ی خارجی ندارند.
* پروژه برای **Dynamics CRM 2016** ساخته شده است (با Dynamics CRM SDK نسخه‌ی 8.2 و .NET Framework 4.6.1).
* برای نصب فقط به فایل **Managed Solution** و یک کاربر با نقش **System Administrator** (یا دسترسی معادل برای Import کردن Solution) نیاز دارید. نیازی به Visual Studio، کامپایل کد یا Plugin Registration Tool نیست.
* برای تنظیم گروه و نام استپ‌ها (اختیاری): Windows PowerShell 5.1 و ماژول `Microsoft.Xrm.Tooling.CrmConnector.PowerShell`.

<a id="install"></a>

## نصب و راه‌اندازی

1. **Import کردن Solution:** در CRM به **Settings > Solutions** بروید، **Import** را بزنید، فایل Managed Solution (فایل `.zip`) را انتخاب کنید و مراحل را با **Next** تا **Import** ادامه دهید. بعد از پایان کار، پیام موفقیت را ببینید و **Close** را بزنید.
2. **بررسی نصب:** در **Settings > Processes** یک Workflow بسازید یا باز کنید و **Add Step** را بزنید؛ استپ‌های این مجموعه باید در فهرست دیده شوند (بخش [استفاده در Workflow](#usage)). برای به‌روزرسانی به نسخه‌ی جدید، Solution نسخه‌ی جدید را به همین روش Import کنید.
3. **(اختیاری) گروه‌بندی استپ‌ها:** فایل `.env.example` را در ریشه‌ی پروژه به `.env` کپی کنید و رشته‌ی اتصال CRM را در `CRM_CONNECTION_STRING` بنویسید. سپس از پوشه‌ی `MvcTeam.Utilities\Deploy` اسکریپت `Set-WorkflowGroups.ps1` را اجرا کنید (با `-WhatIf` می‌توانید ابتدا فقط پیش‌نمایش ببینید). این اسکریپت گروه و نام نمایشی استپ‌ها را در منوی Add Step تنظیم می‌کند.

> **نکته:** `.env` حاوی اطلاعات اتصال است و نباید در Git نگه داشته شود. رمز عبور و کلیدها را هرگز در کد ننویسید.

<a id="usage"></a>

## استفاده در Workflow

1. در CRM به **Settings > Processes** بروید و یک Workflow بسازید یا باز کنید.
2. **Add Step** را بزنید. استپ‌ها زیر گروه‌های **MvcTeam Persian**، **MvcTeam Utilities**، **MvcTeam Security** و **MvcTeam Date & Time** دیده می‌شوند (استپ‌هایی که در اسکریپت گروه‌بندی نیستند، زیر گروه پیش‌فرضی که با ثبت اسمبلی ساخته می‌شود می‌آیند).
3. استپ را انتخاب و با **Set Properties** پارامترها را پر کنید. خروجی‌های استپ در استپ‌های بعدی از **Form Assistant** در دسترس‌اند.
4. برای جزئیات هر استپ، صفحه‌ی آن را در فهرست پایین باز کنید.

<a id="notes"></a>

## نکات مهم پیش از شروع

* **تاریخ‌ها در CRM به‌صورت UTC هستند.** استپ‌های گروه Persian و استپ‌های روز کاری ایران تاریخ را به **وقت ایران** تبدیل می‌کنند؛ بعضی استپ‌های دیگر با گزینه‌ی `Evaluate As User Local` از منطقه‌ی زمانی کاربر استفاده می‌کنند. در صفحه‌ی هر استپ توضیح داده شده است.
* **دو خانواده‌ی روز کاری داریم:** [Add Business Days](docs/datetime/add-business-days.md) و [Business Days Between](docs/datetime/business-days-between.md) آخر هفته را خودتان تعیین می‌کنید (پیش‌فرض پنج‌شنبه و جمعه)؛ ولی استپ‌های `Is Business Day` و `Date Diff Business …` آخر هفته را همیشه شنبه و یکشنبه می‌دانند. برای تقویم ایران از دسته‌ی اول استفاده کنید.
* **رقم‌های فارسی:** استپ‌های تبدیل متن به عدد (مثل [To Decimal](docs/numeric/to-decimal.md)) رقم فارسی را عدد نمی‌شناسند؛ ولی [Validate National Code](docs/persian/validate-national-code.md) ارقام فارسی را هم می‌پذیرد.
* **سطح دسترسی:** بیشتر استپ‌ها با دسترسی کاربر اجراکننده‌ی Workflow اجرا می‌شوند. چند استپ (مثل بررسی نقش، Recalculate All Rollups و خواندن فهرست تعطیلات) با دسترسی سیستمی کار می‌کنند. جزئیات در صفحه‌ی هر استپ.
* **زبان سرور:** چند استپ (مثل [Get Formatted Date String](docs/datetime/get-formatted-date-string.md)، [Get Week Number Of Year](docs/datetime/get-week-number-of-year.md) و [To Date Time](docs/datetime/to-date-time.md)) به تنظیمات زبان و منطقه‌ی Windows سرور وابسته‌اند.
* **مقدارهای پیش‌فرض** پارامترها (مثل `-1` در Round Decimal Places) در طراحی Workflow اعمال می‌شوند.

<a id="steps"></a>

## فهرست همه‌ی استپ‌ها

| دسته | تعداد | شامل |
|---|---|---|
| [ابزارهای فارسی](#persian) | 4 | تبدیل عدد به حروف فارسی، نمایش و تجزیه‌ی تاریخ شمسی و اعتبارسنجی کد ملی؛ هسته‌ی این مجموعه برای کاربران ایرانی. |
| [تاریخ و زمان](#datetime) | 47 | جمع و تفریق تاریخ، اختلاف دو تاریخ، اجزای تاریخ، شروع و پایان بازه‌ها، گرد کردن زمان، روز کاری (با آخر هفته‌ی ایران) و تبدیل‌ها. |
| [متن](#string) | 31 | عملیات روی متن: جستجو، جایگزینی، Regular Expression، Base64 و کدگذاری HTML و URL، پُر کردن، برش، ساخت متن تصادفی و بیشتر. |
| [اعداد](#numeric) | 18 | حساب چهارعمل، گرد کردن، ریشه و توان، عدد تصادفی، تبدیل متن به عدد و عدد به حروف انگلیسی. |
| [ایمیل](#email) | 15 | افزودن گیرنده‌ها (To و CC) از تیم، نقش امنیتی، واحد سازمانی، صف و Connection؛ ارسال ایمیل، بررسی و حذف پیوست‌ها و ساخت قالب. |
| [یادداشت و پیوست](#note) | 10 | کپی، جابه‌جایی، حذف و ویرایش یادداشت‌ها (Note) و مدیریت فایل‌های پیوست آن‌ها. |
| [امنیت و دسترسی](#security) | 5 | بررسی نقش امنیتی و عضویت در تیم، گرفتن کاربر شروع‌کننده و تغییر تنظیمات کاربر. |
| [ابزارهای عمومی](#utilities) | 16 | کلون رکورد، گرفتن نتیجه‌ی جستجو به‌صورت جدول، اجرای Workflow روی نتایج، Rollup، JSON، تبدیل ارز و ابزارهای دیگر. |

<a id="persian"></a>

### ابزارهای فارسی (4 استپ)

تبدیل عدد به حروف فارسی، نمایش و تجزیه‌ی تاریخ شمسی و اعتبارسنجی کد ملی؛ هسته‌ی این مجموعه برای کاربران ایرانی.

| استپ | توضیح |
|---|---|
| [Convert Number To Persian String](docs/persian/convert-number-to-persian-string.md) | عدد را به حروف فارسی می‌نویسد؛ مثلاً «دوازده هزار و پانصد». از عدد منفی و اعشاری هم پشتیبانی می‌کند. |
| [Format Persian DateTime](docs/persian/format-persian-date-time.md) | تاریخ میلادی را با قالب دلخواه به تاریخ شمسی (جلالی) و به وقت ایران تبدیل می‌کند؛ با ارقام فارسی یا انگلیسی. |
| [Get Date Persian Parts](docs/persian/get-date-persian-parts.md) | سال، ماه، روز، فصل و چند شکل آماده‌ی تاریخ شمسی را جدا برمی‌گرداند. |
| [Validate National Code](docs/persian/validate-national-code.md) | ساختار و رقم کنترل کد ملی ۱۰ رقمی ایرانی را بررسی می‌کند. |

<a id="datetime"></a>

### تاریخ و زمان (47 استپ)

جمع و تفریق تاریخ، اختلاف دو تاریخ، اجزای تاریخ، شروع و پایان بازه‌ها، گرد کردن زمان، روز کاری (با آخر هفته‌ی ایران) و تبدیل‌ها.

#### افزودن و کم کردن

| استپ | توضیح |
|---|---|
| [Add Days](docs/datetime/add-days.md) | تعداد مشخصی روز به یک تاریخ اضافه می‌کند (یا با عدد منفی از آن کم می‌کند). |
| [Add Weeks](docs/datetime/add-weeks.md) | تعداد مشخصی هفته (هر هفته ۷ روز) به یک تاریخ اضافه می‌کند یا از آن کم می‌کند. |
| [Add Months](docs/datetime/add-months.md) | تعداد مشخصی ماه میلادی به یک تاریخ اضافه می‌کند یا از آن کم می‌کند. |
| [Add Years](docs/datetime/add-years.md) | تعداد مشخصی سال میلادی به یک تاریخ اضافه می‌کند یا از آن کم می‌کند. |
| [Add Hours](docs/datetime/add-hours.md) | تعداد مشخصی ساعت به یک تاریخ و زمان اضافه می‌کند یا از آن کم می‌کند. |
| [Add Minutes](docs/datetime/add-minutes.md) | تعداد مشخصی دقیقه به یک تاریخ و زمان اضافه می‌کند یا از آن کم می‌کند. |

#### روز کاری با آخر هفته‌ی قابل تنظیم (مناسب تقویم ایران)

| استپ | توضیح |
|---|---|
| [Add Business Days](docs/datetime/add-business-days.md) | تعداد مشخصی روز کاری به یک تاریخ اضافه می‌کند؛ آخر هفته (پیش‌فرض پنج‌شنبه و جمعه) و تعطیلات را نمی‌شمارد و روزها به وقت ایران حساب می‌شوند. |
| [Business Days Between](docs/datetime/business-days-between.md) | فاصله‌ی دو تاریخ را بر حسب روز کاری، روز تقویمی، ساعت و دقیقه برمی‌گرداند؛ با آخر هفته‌ی قابل تنظیم (پیش‌فرض پنج‌شنبه و جمعه) و وقت ایران. |

#### روز کاری بر اساس تقویم CRM (آخر هفته: شنبه و یکشنبه)

| استپ | توضیح |
|---|---|
| [Is Business Day](docs/datetime/is-business-day.md) | بررسی می‌کند یک تاریخ روز کاری است یا نه (بر اساس تقویم CRM؛ شنبه و یکشنبه آخر هفته حساب می‌شوند). |
| [Get Number Of Business Days](docs/datetime/get-number-of-business-days.md) | تعداد روزهای کاری یک بازه را می‌شمارد (بر اساس تقویم CRM؛ هر دو سر بازه شامل). |
| [Date Diff Business Days](docs/datetime/date-diff-business-days.md) | تعداد روزهای کاریِ کامل بین دو تاریخ را برمی‌گرداند. |
| [Date Diff Business Hours](docs/datetime/date-diff-business-hours.md) | تعداد ساعت‌هایی از بازه‌ی دو تاریخ را برمی‌گرداند که در روزهای کاری قرار دارند. |
| [Date Diff Business Minutes](docs/datetime/date-diff-business-minutes.md) | تعداد دقیقه‌هایی از بازه‌ی دو تاریخ را برمی‌گرداند که در روزهای کاری قرار دارند. |

#### اختلاف دو تاریخ

| استپ | توضیح |
|---|---|
| [Date Diff](docs/datetime/date-diff.md) | اختلاف دو تاریخ را به‌صورت متن خوانا برمی‌گرداند، مثلاً `2d.03h:20m`. |
| [Date Diff Days](docs/datetime/date-diff-days.md) | اختلاف دو تاریخ را بر حسب روز (عدد صحیح و همیشه نامنفی) برمی‌گرداند. |
| [Date Diff Hours](docs/datetime/date-diff-hours.md) | اختلاف دو تاریخ را بر حسب ساعت (عدد صحیح و همیشه نامنفی) برمی‌گرداند. |
| [Date Diff Minutes](docs/datetime/date-diff-minutes.md) | اختلاف دو تاریخ را بر حسب دقیقه (عدد صحیح و همیشه نامنفی) برمی‌گرداند. |
| [Date Diff Seconds](docs/datetime/date-diff-seconds.md) | اختلاف دو تاریخ را بر حسب ثانیه (عدد صحیح و همیشه نامنفی) برمی‌گرداند. |
| [Date Diff Months](docs/datetime/date-diff-months.md) | تعداد ماه‌های میلادیِ کامل بین دو تاریخ را برمی‌گرداند (عدد صحیح و همیشه نامنفی). |
| [Date Diff Years](docs/datetime/date-diff-years.md) | تعداد سال‌های میلادیِ کامل بین دو تاریخ را برمی‌گرداند (عدد صحیح و همیشه نامنفی)؛ برای مثال محاسبه‌ی سن یا سابقه. |
| [Relative Time](docs/datetime/relative-time.md) | فاصله‌ی دو تاریخ را به‌صورت یک عبارت انگلیسی مثل `3 hours ago` یا `yesterday` برمی‌گرداند. |

#### گرفتن اجزای تاریخ

| استپ | توضیح |
|---|---|
| [Get Year Number](docs/datetime/get-year-number.md) | شماره‌ی سال میلادی یک تاریخ را برمی‌گرداند. |
| [Get Month Number](docs/datetime/get-month-number.md) | شماره‌ی ماه میلادی یک تاریخ را برمی‌گرداند (`1` تا `12`). |
| [Get Month Name](docs/datetime/get-month-name.md) | نام ماه یک تاریخ را برمی‌گرداند و زبان (Culture) را خودتان انتخاب می‌کنید. |
| [Get Day Number](docs/datetime/get-day-number.md) | شماره‌ی روز ماه میلادی یک تاریخ را برمی‌گرداند (`1` تا `31`). |
| [Get Day Number Of Week](docs/datetime/get-day-number-of-week.md) | شماره‌ی روز هفته را برمی‌گرداند؛ شماره‌گذاری از یکشنبه (`1`) تا شنبه (`7`) است. |
| [Get Day Of Week](docs/datetime/get-day-of-week.md) | نام روز هفته را به انگلیسی برمی‌گرداند: `Sunday`، `Monday` … `Saturday`. |
| [Get Day Number Of Year](docs/datetime/get-day-number-of-year.md) | شماره‌ی روز در سال میلادی را برمی‌گرداند: ۱ ژانویه `1` و ۳۱ دسامبر `365` (یا `366` در سال کبیسه). |
| [Get Week Number Of Year](docs/datetime/get-week-number-of-year.md) | شماره‌ی هفته‌ی یک تاریخ را در سال میلادی برمی‌گرداند. |
| [Get Quarter Number Of Year](docs/datetime/get-quarter-number-of-year.md) | شماره‌ی فصل (سه‌ماهه) میلادی یک تاریخ را برمی‌گرداند: ژانویه تا مارس `1`، آوریل تا ژوئن `2`، ژوئیه تا سپتامبر `3` و اکتبر تا دسامبر `4`. |
| [Get Hour Number](docs/datetime/get-hour-number.md) | ساعت (۲۴ ساعته، `0` تا `23`) یک تاریخ و زمان را برمی‌گرداند. |
| [Get Minute Number](docs/datetime/get-minute-number.md) | دقیقه (`0` تا `59`) یک تاریخ و زمان را برمی‌گرداند. |
| [Get Second Number](docs/datetime/get-second-number.md) | ثانیه (`0` تا `59`) یک تاریخ و زمان را برمی‌گرداند. |

#### شروع و پایان بازه‌ها

| استپ | توضیح |
|---|---|
| [Get Week Start End](docs/datetime/get-week-start-end.md) | اولین و آخرین لحظه‌ی هفته‌ی یک تاریخ را برمی‌گرداند. |
| [Get Month Start End](docs/datetime/get-month-start-end.md) | اولین و آخرین لحظه‌ی ماه میلادی یک تاریخ را برمی‌گرداند. |
| [Get Quarter Start End](docs/datetime/get-quarter-start-end.md) | اولین و آخرین لحظه‌ی فصل (سه‌ماهه‌ی) میلادی یک تاریخ را برمی‌گرداند. |
| [Get Year Start End](docs/datetime/get-year-start-end.md) | اولین و آخرین لحظه‌ی سال میلادی یک تاریخ را برمی‌گرداند. |

#### گرد کردن و تغییر زمان

| استپ | توضیح |
|---|---|
| [Round To Hour](docs/datetime/round-to-hour.md) | یک زمان را به نزدیک‌ترین ساعت کامل به بالا یا به پایین گرد می‌کند (جهت را خودتان تعیین می‌کنید). |
| [Round To Half Hour](docs/datetime/round-to-half-hour.md) | یک زمان را به نزدیک‌ترین «نیم ساعت» (دقیقه‌ی `00` یا `30`) به بالا یا به پایین گرد می‌کند. |
| [Round To Quarter Hour](docs/datetime/round-to-quarter-hour.md) | یک زمان را به نزدیک‌ترین «ربع ساعت» (دقیقه‌ی `00`، `15`، `30` یا `45`) به بالا یا به پایین گرد می‌کند. |
| [Set Date Part](docs/datetime/set-date-part.md) | فقط یک بخش از یک تاریخ (ساعت، دقیقه، ثانیه، ماه، روز یا سال میلادی) را با مقدار جدید عوض می‌کند و بقیه‌ی بخش‌ها را نگه می‌دارد. |
| [Set Time](docs/datetime/set-time.md) | ساعت، دقیقه و ثانیه‌ی یک تاریخ را با مقدارهای دلخواه جایگزین می‌کند و روز را نگه می‌دارد. |

#### مقایسه و بررسی

| استپ | توضیح |
|---|---|
| [Is Between](docs/datetime/is-between.md) | بررسی می‌کند که یک تاریخ دقیقاً بین دو تاریخ دیگر قرار دارد یا نه. |
| [Is Same Day](docs/datetime/is-same-day.md) | بررسی می‌کند که دو تاریخ در یک روز تقویمی (میلادی) هستند یا نه، صرف‌نظر از ساعت. |

#### تبدیل به متن و از متن

| استپ | توضیح |
|---|---|
| [Get Formatted Date String](docs/datetime/get-formatted-date-string.md) | یک تاریخ را با قالب دلخواه (.NET) به متن تبدیل می‌کند؛ نتیجه به زبان (Culture) سرور بستگی دارد. |
| [To UTC String](docs/datetime/to-utc-string.md) | تاریخ را به UTC تبدیل و با قالب استاندارد زبان (Culture) انتخاب‌شده به متن تبدیل می‌کند. |
| [To Date Time](docs/datetime/to-date-time.md) | یک متن را به تاریخ و زمان تبدیل می‌کند و مشخص می‌کند تبدیل موفق بود یا نه. |

<a id="string"></a>

### متن (31 استپ)

عملیات روی متن: جستجو، جایگزینی، Regular Expression، Base64 و کدگذاری HTML و URL، پُر کردن، برش، ساخت متن تصادفی و بیشتر.

| استپ | توضیح |
|---|---|
| [B64 Decode](docs/string/b64-decode.md) | یک متن Base64 را به متن اصلی برمی‌گرداند. |
| [B64 Encode](docs/string/b64-encode.md) | یک متن را به Base64 تبدیل می‌کند. |
| [Contains](docs/string/contains.md) | بررسی می‌کند که یک متن شامل متن دیگری هست یا نه. |
| [Create Empty Spaces](docs/string/create-empty-spaces.md) | متنی می‌سازد که فقط از تعداد مشخصی فاصله (space) تشکیل شده است؛ برای ساخت فاصله بین دو بخش از متن. |
| [Decode Html](docs/string/decode-html.md) | نویسه‌های کدگذاری‌شده‌ی HTML را به نویسه‌ی اصلی برمی‌گرداند (`&lt;` ← `<`، `&amp;` ← `&` و مانند آن). |
| [Encode Html](docs/string/encode-html.md) | نویسه‌های خاص HTML را در یک متن به معادل امن آن‌ها تبدیل می‌کند (`<` ← `&lt;`، `>` ← `&gt;`، `&` ← `&amp;` و مانند آن). |
| [Ends With](docs/string/ends-with.md) | بررسی می‌کند که یک متن با متن دیگری تمام می‌شود یا نه؛ مثلاً برای بررسی پسوند نام فایل. |
| [Join](docs/string/join.md) | دو متن را به هم وصل می‌کند و در صورت نیاز یک جداکننده بین آن‌ها می‌گذارد. |
| [Length](docs/string/length.md) | تعداد نویسه‌های یک متن را برمی‌گرداند. |
| [Pad Left](docs/string/pad-left.md) | یک نویسه (یا متن) را به تعداد مشخص در ابتدای متن تکرار می‌کند. |
| [Pad Left Dynamic](docs/string/pad-left-dynamic.md) | ابتدای یک متن را با Pad Character پُر می‌کند تا متن به طول نهایی مشخص برسد؛ مثلاً برای ساخت کد با صفرِ ابتدایی. |
| [Pad Right](docs/string/pad-right.md) | یک نویسه (یا متن) را به تعداد مشخص در انتهای متن تکرار می‌کند. |
| [Pad Right Dynamic](docs/string/pad-right-dynamic.md) | انتهای یک متن را با Pad Character پُر می‌کند تا متن به طول نهایی مشخص برسد. |
| [Random](docs/string/random.md) | یک متن تصادفی با طول مشخص می‌سازد. |
| [Regex Extract](docs/string/regex-extract.md) | اولین بخشی از متن را که با یک Regular Expression هم‌خوانی دارد برمی‌گرداند. |
| [Regex Match](docs/string/regex-match.md) | بررسی می‌کند که یک متن با یک Regular Expression هم‌خوانی دارد یا نه. |
| [Regex Replace](docs/string/regex-replace.md) | همه‌ی بخش‌های هم‌خوان با یک Regular Expression را با یک متن جایگزین می‌کند. |
| [Regex Replace With Space](docs/string/regex-replace-with-space.md) | همه‌ی بخش‌های هم‌خوان با یک Regular Expression را با تعداد مشخصی فاصله (space) جایگزین می‌کند. |
| [Remove Html](docs/string/remove-html.md) | تگ‌های HTML را از یک متن حذف می‌کند و فقط متن را نگه می‌دارد. |
| [Replace](docs/string/replace.md) | همه‌ی موارد یک متن را در متن دیگر با متن جدید جایگزین می‌کند. |
| [Replace With Space](docs/string/replace-with-space.md) | همه‌ی موارد یک متن را با تعداد مشخصی فاصله (space) جایگزین می‌کند. |
| [Reverse](docs/string/reverse.md) | ترتیب نویسه‌های یک متن را برعکس می‌کند. |
| [Starts With](docs/string/starts-with.md) | بررسی می‌کند که یک متن با متن دیگری شروع می‌شود یا نه. |
| [Substring](docs/string/substring.md) | بخشی از یک متن را از یک موقعیت مشخص و با طول مشخص جدا می‌کند. |
| [To Lower](docs/string/to-lower.md) | همه‌ی حروف یک متن را به حروف کوچک تبدیل می‌کند. |
| [To Title Case](docs/string/to-title-case.md) | حرف اول هر کلمه را بزرگ می‌کند (Title Case). |
| [To Upper](docs/string/to-upper.md) | همه‌ی حروف یک متن را به حروف بزرگ تبدیل می‌کند. |
| [Trim](docs/string/trim.md) | فاصله‌های (space) ابتدا و انتهای یک متن را حذف می‌کند. |
| [Url Decode](docs/string/url-decode.md) | متنی را که برای آدرس اینترنتی کدگذاری شده (نویسه‌هایی مثل `%20` و `%26`) به متن اصلی برمی‌گرداند. |
| [Url Encode](docs/string/url-encode.md) | یک متن را طوری کدگذاری می‌کند که بتوان آن را با امنیت به‌عنوان بخشی از آدرس اینترنتی (URL) به کار برد. |
| [Word Count](docs/string/word-count.md) | تعداد کلمه‌های یک متن را می‌شمارد. |

<a id="numeric"></a>

### اعداد (18 استپ)

حساب چهارعمل، گرد کردن، ریشه و توان، عدد تصادفی، تبدیل متن به عدد و عدد به حروف انگلیسی.

| استپ | توضیح |
|---|---|
| [Abs Value](docs/numeric/abs-value.md) | قدر مطلق یک عدد را برمی‌گرداند؛ یعنی عدد را بدون علامت منفی. |
| [Add](docs/numeric/add.md) | دو عدد را با هم جمع می‌کند و در صورت نیاز حاصل را گرد می‌کند. |
| [Average](docs/numeric/average.md) | میانگین دو عدد را حساب می‌کند و در صورت نیاز حاصل را گرد می‌کند. |
| [Divide](docs/numeric/divide.md) | عدد اول را بر عدد دوم تقسیم می‌کند و در صورت نیاز حاصل را گرد می‌کند. |
| [Integer To Words](docs/numeric/integer-to-words.md) | یک عدد صحیح را به حروف انگلیسی تبدیل می‌کند؛ مثلاً `123` را به `one hundred and twenty-three`. |
| [Is String Numeric](docs/numeric/is-string-numeric.md) | بررسی می‌کند که یک متن قابل تبدیل به عدد هست یا نه. |
| [Max](docs/numeric/max.md) | از بین دو عدد، بزرگ‌تر را برمی‌گرداند. |
| [Min](docs/numeric/min.md) | از بین دو عدد، کوچک‌تر را برمی‌گرداند. |
| [Multiply](docs/numeric/multiply.md) | دو عدد را در هم ضرب می‌کند و در صورت نیاز حاصل را گرد می‌کند. |
| [Nth Root](docs/numeric/nth-root.md) | ریشه‌ی n‌ام یک عدد را حساب می‌کند؛ مثلاً ریشه‌ی دوم (جذر) یا ریشه‌ی سوم. |
| [Raise To The Power](docs/numeric/raise-to-the-power.md) | یک عدد را به توان عدد دیگر می‌رساند. |
| [Random Number](docs/numeric/random-number.md) | یک عدد صحیح تصادفی از `0` تا کمتر از مقدار داده‌شده می‌سازد. |
| [Random Number Between](docs/numeric/random-number-between.md) | یک عدد صحیح تصادفی بین حد پایین و حد بالا می‌سازد. |
| [Round](docs/numeric/round.md) | یک عدد را تا تعداد رقم اعشار مشخص گرد می‌کند. |
| [Subtract](docs/numeric/subtract.md) | عدد دوم را از عدد اول کم می‌کند و در صورت نیاز حاصل را گرد می‌کند. |
| [To Decimal](docs/numeric/to-decimal.md) | یک متن را به عدد اعشاری (Decimal) تبدیل می‌کند و مشخص می‌کند تبدیل موفق بود یا نه. |
| [To Integer](docs/numeric/to-integer.md) | یک متن را به عدد صحیح (Integer) تبدیل می‌کند و مشخص می‌کند تبدیل موفق بود یا نه. |
| [Truncate](docs/numeric/truncate.md) | اعشار یک عدد را از تعداد رقم مشخصی به بعد می‌برد، بدون اینکه گرد کند. |

<a id="email"></a>

### ایمیل (15 استپ)

افزودن گیرنده‌ها (To و CC) از تیم، نقش امنیتی، واحد سازمانی، صف و Connection؛ ارسال ایمیل، بررسی و حذف پیوست‌ها و ساخت قالب.

#### افزودن گیرنده به خط To

| استپ | توضیح |
|---|---|
| [Email Team](docs/email/email-team.md) | اعضای یک تیم (Team) را به خط To (گیرنده) یک ایمیل اضافه می‌کند و در صورت نیاز ایمیل را می‌فرستد. |
| [Email Security Role](docs/email/email-security-role.md) | همه‌ی کاربرانی را که یک نقش امنیتی (Security Role) دارند به خط To (گیرنده) یک ایمیل اضافه می‌کند و در صورت نیاز ایمیل را می‌فرستد. |
| [Email Business Unit](docs/email/email-business-unit.md) | کاربران یک واحد سازمانی (Business Unit) را به خط To (گیرنده) یک ایمیل اضافه می‌کند و در صورت نیاز ایمیل را می‌فرستد. |
| [Email Queue Members](docs/email/email-queue-members.md) | اعضای یک صف (Queue) را به خط To (گیرنده) یک ایمیل اضافه می‌کند و در صورت نیاز ایمیل را می‌فرستد. |
| [Email Connection](docs/email/email-connection.md) | رکوردهای مرتبط با رکورد اصلیِ Workflow از طریق Connection (با نقش ارتباط مشخص) را به خط To ایمیل اضافه می‌کند. |

#### افزودن گیرنده به خط CC

| استپ | توضیح |
|---|---|
| [Cc Team](docs/email/cc-team.md) | اعضای یک تیم (Team) را به خط CC (رونوشت) یک ایمیل اضافه می‌کند و در صورت نیاز ایمیل را می‌فرستد. |
| [Cc Security Role](docs/email/cc-security-role.md) | همه‌ی کاربرانی را که یک نقش امنیتی (Security Role) دارند به خط CC (رونوشت) یک ایمیل اضافه می‌کند. |
| [Cc Business Unit](docs/email/cc-business-unit.md) | کاربران یک واحد سازمانی (Business Unit) را به خط CC (رونوشت) یک ایمیل اضافه می‌کند. |
| [Cc Queue Members](docs/email/cc-queue-members.md) | اعضای یک صف (Queue) را به خط CC (رونوشت) یک ایمیل اضافه می‌کند. |
| [Cc Connection](docs/email/cc-connection.md) | رکوردهای مرتبط با رکورد اصلیِ Workflow از طریق Connection (با نقش ارتباط مشخص) را به خط CC ایمیل اضافه می‌کند. |

#### ارسال، قالب و پیوست‌ها

| استپ | توضیح |
|---|---|
| [Send Email](docs/email/send-email.md) | یک ایمیل موجود (پیش‌نویس) را می‌فرستد. |
| [Create Template](docs/email/create-template.md) | یک قالب ایمیل (Email Template) را برای یک رکورد مشخص پر می‌کند و موضوع و متن حاصل را برمی‌گرداند. |
| [Check Attachments](docs/email/check-attachments.md) | بررسی می‌کند که یک ایمیل فایل پیوست دارد یا نه و تعداد پیوست‌ها را برمی‌گرداند. |
| [Delete Email Attachment](docs/email/delete-email-attachment.md) | فایل‌های پیوست یک ایمیل را در صورتی که با شرط‌های حجم (و در صورت نیاز پسوند فایل) هم‌خوانی داشته باشند حذف می‌کند. |
| [Delete Email Attachment By Name](docs/email/delete-email-attachment-by-name.md) | پیوست‌های یک ایمیل را که نام فایل آن‌ها با نام داده‌شده یکی است حذف می‌کند. |

<a id="note"></a>

### یادداشت و پیوست (10 استپ)

کپی، جابه‌جایی، حذف و ویرایش یادداشت‌ها (Note) و مدیریت فایل‌های پیوست آن‌ها.

| استپ | توضیح |
|---|---|
| [Check Attachment](docs/note/check-attachment.md) | بررسی می‌کند که یک یادداشت (Note) فایل پیوست دارد یا نه، و نتیجه را به‌صورت `True` یا `False` برمی‌گرداند. |
| [Copy Note](docs/note/copy-note.md) | یک یادداشت (Note) را زیر رکورد دیگری کپی می‌کند. |
| [Delete Attachment](docs/note/delete-attachment.md) | فایل پیوست یک یادداشت (Note) را در صورتی که با شرط‌های حجم (و در صورت نیاز پسوند فایل) هم‌خوانی داشته باشد حذف می‌کند. |
| [Delete Attachment By Name](docs/note/delete-attachment-by-name.md) | فایل پیوست یک یادداشت (Note) را در صورتی که نام فایل آن با نام داده‌شده یکی باشد حذف می‌کند. |
| [Delete Note](docs/note/delete-note.md) | یک یادداشت (Note) را به‌طور کامل حذف می‌کند: متن، عنوان و فایل پیوست آن. |
| [Get Latest Note](docs/note/get-latest-note.md) | جدیدترین یادداشت (Note) یک رکورد را پیدا می‌کند و آن را به‌عنوان یک Lookup برمی‌گرداند. |
| [Get Latest Note By Filename](docs/note/get-latest-note-by-filename.md) | در یادداشت‌های (Note) یک رکورد، یادداشتی را پیدا می‌کند که فایل پیوست آن نام مشخصی دارد. |
| [Move Note](docs/note/move-note.md) | یک یادداشت (Note) را از رکوردی که به آن مربوط است (Regarding) به رکورد دیگری منتقل می‌کند. |
| [Update Note Text](docs/note/update-note-text.md) | متن یک یادداشت (Note) را با متن جدید جایگزین می‌کند. |
| [Update Note Title](docs/note/update-note-title.md) | عنوان (Title) یک یادداشت (Note) را با عنوان جدید جایگزین می‌کند. |

<a id="security"></a>

### امنیت و دسترسی (5 استپ)

بررسی نقش امنیتی و عضویت در تیم، گرفتن کاربر شروع‌کننده و تغییر تنظیمات کاربر.

| استپ | توضیح |
|---|---|
| [Get Initiating User](docs/security/get-initiating-user.md) | کاربری را برمی‌گرداند که اجرای Workflow را در ابتدا شروع کرده است. |
| [Is User Has Role](docs/security/is-user-has-role.md) | بررسی می‌کند که یک کاربر یک نقش امنیتی (Security Role) مشخص را به‌طور مستقیم دارد یا نه. |
| [Is User Member Of Team](docs/security/is-user-member-of-team.md) | بررسی می‌کند که یک کاربر عضو یک تیم (Team) مشخص هست یا نه. |
| [Is User Teams Have Role](docs/security/is-user-teams-have-role.md) | بررسی می‌کند که حداقل یکی از تیم‌هایی که کاربر عضو آن‌هاست یک نقش امنیتی (Security Role) مشخص را دارد یا نه. |
| [Set User Settings](docs/security/set-user-settings.md) | تنظیمات شخصی یک کاربر را تغییر می‌دهد: تعداد رکورد در هر صفحه، حالت Advanced Find، منطقه‌ی زمانی، زبان و نمای تقویم. |

<a id="utilities"></a>

### ابزارهای عمومی (16 استپ)

کلون رکورد، گرفتن نتیجه‌ی جستجو به‌صورت جدول، اجرای Workflow روی نتایج، Rollup، JSON، تبدیل ارز و ابزارهای دیگر.

| استپ | توضیح |
|---|---|
| [Add Price List Item](docs/utilities/add-price-list-item.md) | یک آیتم قیمت (Price List Item) جدید می‌سازد: یک محصول را با یک واحد اندازه‌گیری و یک قیمت مشخص به یک لیست قیمت اضافه می‌کند. |
| [Clone Children](docs/utilities/clone-children.md) | رکوردهای فرزند (Child) یک رکورد را از طریق یک رابطه کلون می‌کند و به رکورد والد جدید وصل می‌کند. |
| [Clone Record](docs/utilities/clone-record.md) | از یک رکورد یک کپی (با همان مقدارها) می‌سازد. |
| [Currency Convert](docs/utilities/currency-convert.md) | یک مبلغ را از یک ارز به ارز دیگر تبدیل می‌کند. |
| [Delete Record](docs/utilities/delete-record.md) | یک رکورد را حذف می‌کند. |
| [Get Record Id](docs/utilities/get-record-id.md) | از روی Record URL (Dynamic) یک رکورد، شناسه‌ی یکتا (GUID) و نام نوع موجودیت آن را جدا می‌کند و به‌صورت دو متن برمی‌گرداند. |
| [JSON Parser](docs/utilities/json-parser.md) | یک مقدار را با مسیر (Path) از داخل یک متن JSON می‌خواند. |
| [Query Get Results](docs/utilities/query-get-results.md) | یک View یا FetchXml را اجرا می‌کند و نتیجه را به‌صورت جدول HTML، CSV، لیست و مقدار تکی برمی‌گرداند. |
| [Query Run Workflow On Results](docs/utilities/query-run-workflow-on-results.md) | یک Workflow را روی هر رکوردی اجرا می‌کند که یک View یا یک FetchXml برمی‌گرداند. |
| [Query Values](docs/utilities/query-values.md) | در یک موجودیت با یک یا دو شرط جستجو می‌کند و یک یا دو فیلد رکورد پیداشده را برمی‌گرداند. |
| [Recalculate All Rollups](docs/utilities/recalculate-all-rollups.md) | همه‌ی فیلدهای Rollup منتشرشده‌ی یک رکورد را دوباره محاسبه می‌کند، بدون اینکه منتظر Job زمان‌بندی‌شده‌ی CRM بمانید. |
| [Resize Annotation Image](docs/utilities/resize-annotation-image.md) | تصویر پیوست یک یادداشت (Note) را کوچک و فشرده می‌کند تا حجم فایل‌ها در CRM کمتر شود. |
| [Run Workflow With For Loop](docs/utilities/run-workflow-with-for-loop.md) | یک Workflow دیگر را به تعداد مشخص روی یک رکورد اجرا می‌کند (مثل یک حلقه‌ی `for`). |
| [String Functions](docs/utilities/string-functions.md) | چند عملیات رایج روی متن را یک‌جا انجام می‌دهد: بزرگ‌کردن حرف اول، پُر کردن، جایگزینی، Substring، Regular Expression، حروف بزرگ و کوچک و حذف فاصله. |
| [Update Field Dynamically](docs/utilities/update-field-dynamically.md) | یک فیلد مشخص از یک رکورد را با مقدار جدید به‌روز می‌کند؛ در حالی که هم رکورد و هم نام فیلد را به‌صورت داینامیک (به‌شکل متن) به آن می‌دهید. |
| [Workflow Add Signature To Email](docs/utilities/workflow-add-signature-to-email.md) | امضای مالک ایمیل را به انتهای متن ایمیل اضافه می‌کند. مخصوص یک سازمان مشخص است و به فیلد سفارشی `new_signature` نیاز دارد. |

<a id="layout"></a>

## ساختار پوشه‌ها

```
MvcXrmTools/
├── README.md                      ← همین صفحه
├── docs/                          ← مستندات فارسی، یک صفحه برای هر استپ
│   ├── persian/  datetime/  string/  numeric/
│   ├── email/    note/      security/ utilities/
│   └── <دسته>/images/             ← تصاویر مستندات
├── MvcTeam.Utilities/             ← پروژه‌ی اصلی (استپ‌ها در Workflows/<دسته>)
│   └── Deploy/Set-WorkflowGroups.ps1
├── MvcTeam.Utilities.Core/        ← کدهای مشترک (Shared Project)
└── MvcTeam.Utilities.sln
```

<a id="credits"></a>

## منابع و قدردانی

بخشی از کد این مجموعه از پروژه‌های متن‌باز زیر گرفته شده و برای Dynamics CRM 2016 سازگار و اصلاح شده است. از نویسندگان آن‌ها سپاسگزاریم. ستون‌های خالی هنوز تکمیل نشده‌اند.

| پروژه‌ی مبدأ | نویسنده | مجوز | مخزن | استپ‌های مرتبط در این مجموعه |
|---|---|---|---|---|
| Dynamics-365-Workflow-Tools | Demian Rasko | Ms-PL | https://github.com/demianrasko/Dynamics-365-Workflow-Tools | Clone Record، Clone Children، Currency Convert (بازنویسی‌شده)، Delete Record، Get Initiating User، JSON Parser، Query Values، Set User Settings، String Functions |
| CRM-Email-Workflow-Utilities | Jason Lattimer | MIT | https://github.com/jlattimer/CRM-Email-Workflow-Utilities | دسته‌ی [ایمیل](#email) |
| WorkflowElements | Aiden Kaskela | MIT | https://github.com/akaskela/WorkflowElements | Query Get Results، Query Run Workflow On Results |
| LAT Workflow Utilities (Numeric) | | | | دسته‌ی [اعداد](#numeric) |
| | | | | دسته‌ی [متن](#string) |
| | | | | دسته‌ی [یادداشت و پیوست](#note) |
| | | | | دسته‌ی [تاریخ و زمان](#datetime) (به‌جز دو استپ روز کاری ایران) |
| Ultimate Workflow Toolkit | Andrii Butenko | MIT | https://github.com/AndrewButenko/UltimateWorkflowToolkit | |

استپ‌های دسته‌ی [ابزارهای فارسی](#persian) و دو استپ [Add Business Days](docs/datetime/add-business-days.md) و [Business Days Between](docs/datetime/business-days-between.md) برای همین پروژه نوشته شده‌اند.

<a id="license"></a>

## مجوز

این پروژه تحت مجوز **MIT** منتشر شده است؛ یعنی می‌توانید آزادانه و رایگان از آن استفاده کنید، آن را تغییر دهید و منتشر کنید، به شرطی که اعلان حق نشر و متن مجوز همراه آن بماند.

کد گرفته‌شده از پروژه‌های بالا تابع مجوز همان پروژه‌هاست (Ms-PL و MIT) و باید اعلان‌های حق نشر و مجوز آن‌ها حفظ شود. متن کامل این اعلان‌ها در فایل [THIRD-PARTY-NOTICES.md](THIRD-PARTY-NOTICES.md) آمده است.

</div>
