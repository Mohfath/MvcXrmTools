<div dir="rtl">

# Get Latest Note (دریافت آخرین یادداشت)

این استپ جدیدترین یادداشت (Note) یک رکورد را پیدا می‌کند و آن را به‌عنوان یک Lookup برمی‌گرداند. «جدیدترین» یعنی یادداشتی که تاریخ ساخت (Created On) آن از همه دیرتر است.

برای استفاده از این استپ، در طراحی Workflow از منوی **Add Step** استپ **Get Latest Note** را انتخاب کنید.

## پارامترهای ورودی

* **Record Dynamic Url (اجباری)** : آدرس داینامیک رکوردی که یادداشت‌های آن جستجو می‌شود. برای پر کردن آن از مقدار **Record URL (Dynamic)** رکورد موردنظر در Dynamic Values استفاده کنید.

## پارامتر خروجی

* **Found Note** : Lookup به جدیدترین یادداشتِ رکورد. اگر رکورد هیچ یادداشتی نداشته باشد، خالی است.

## نکته‌ها

* استپ فقط جستجو می‌کند و چیزی را تغییر نمی‌دهد. از **Found Note** می‌توانید به‌عنوان ورودی استپ‌های دیگر Note استفاده کنید، مثل [Copy Note](copy-note.md)، [Move Note](move-note.md)، [Update Note Text](update-note-text.md)، [Update Note Title](update-note-title.md) یا [Check Attachment](check-attachment.md).
* چون ممکن است رکورد یادداشتی نداشته باشد، پیش از استفاده از **Found Note** با یک شرط (Check Condition) مطمئن شوید مقدار دارد.
* اگر می‌خواهید یادداشتی را که فایل مشخصی دارد پیدا کنید، از [Get Latest Note By Filename](get-latest-note-by-filename.md) استفاده کنید.
* استپ با دسترسی کاربر اجراکننده‌ی Workflow اجرا می‌شود؛ پس فقط یادداشت‌هایی را پیدا می‌کند که آن کاربر اجازه‌ی خواندنشان را دارد.

## مثال کاربرد

روی یک Case، آخرین یادداشت را پیدا کنید و متن آن را با [Update Note Text](update-note-text.md) تغییر دهید.

</div>
