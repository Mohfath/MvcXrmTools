<div dir="rtl">

<!-- nav-top -->
[« صفحه‌ی اصلی](../../README.md) › [ابزارهای عمومی](../../README.md#utilities)

# گرفتن شناسه و نوع رکورد از Record URL در Dynamics CRM (استپ Get Record Id)

این استپ از روی **Record URL (Dynamic)** یک رکورد، **شناسه‌ی یکتا (GUID)** و **نام نوع موجودیت** آن را جدا می‌کند و به‌صورت دو متن برمی‌گرداند.

برای استفاده از این استپ، در طراحی Workflow از منوی **Add Step** به گروه **MvcTeam Utilities** بروید و **Get Record Id** را انتخاب کنید.

## پارامترهای ورودی

* **Record Reference (اجباری)** : آدرس داینامیک رکورد (**Record URL (Dynamic)**) به‌صورت متن. آن را از Form Assistant انتخاب کنید.

## پارامترهای خروجی

* **Id** : شناسه‌ی رکورد (GUID) به‌صورت متن.
* **Entity Type Name** : نام منطقی (logical name) نوع رکورد، مثلاً `account`.

## نحوه‌ی کار

* آدرس‌های قدیمی که فقط کد نوع موجودیت (`etc`) دارند با یک جستجوی متادیتا به نام موجودیت تبدیل می‌شوند؛ این جستجو با دسترسی **سیستمی** انجام می‌شود.
* اگر آدرس معتبر نباشد یا شناسه نداشته باشد، استپ با خطای `Record Reference '…' is not a valid record URL.` یا `… has no record id.` و اگر نوع موجودیت ناشناخته باشد با `… has an unknown entity type.` متوقف می‌شود. اگر Record Reference خالی باشد، خطای `Record Reference is required.` می‌دهد.

## نکته‌ها

* کاربرد رایج: در استپ‌های شرطی یا استپ‌هایی که به شناسه‌ی متنی رکورد نیاز دارند (مثل [Run Workflow With For Loop](run-workflow-with-for-loop.md) که `Record ID` می‌خواهد) از این استپ استفاده کنید.
* Record URL چیست؟ توضیح آن در [Clone Record](clone-record.md) آمده است.

---

<!-- nav-bottom -->
[⬆ بازگشت به فهرست اصلی و همه‌ی استپ‌ها](../../README.md#steps)

</div>
