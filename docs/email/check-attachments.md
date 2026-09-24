<div dir="rtl">

<!-- nav-top -->
[« صفحه‌ی اصلی](../../README.md) › [ایمیل](../../README.md#email)

# بررسی پیوست‌های ایمیل در Dynamics CRM (استپ Check Attachments)

این استپ بررسی می‌کند که یک ایمیل فایل پیوست دارد یا نه و تعداد پیوست‌ها را برمی‌گرداند.

برای استفاده از این استپ، در طراحی Workflow از منوی **Add Step** استپ **Check Attachments** را انتخاب کنید.

## پارامترهای ورودی

* **Email To Check (اجباری)** : ایمیلی که بررسی می‌شود (Lookup به ایمیل).

## پارامترهای خروجی

* **Has Attachments** : اگر ایمیل حداقل یک پیوست داشته باشد `True` و در غیر این صورت `False` است.
* **Attachment Count** : تعداد پیوست‌های ایمیل.

## مثال

| تعداد پیوست‌ها | Has Attachments | Attachment Count |
|---|---|---|
| `2` | `True` | `2` |
| `0` | `False` | `0` |

## نکته‌ها

* استپ فقط اطلاعات را می‌خواند و چیزی را تغییر نمی‌دهد.
* استپ با دسترسی کاربر اجراکننده‌ی Workflow اجرا می‌شود.
* برای حذف پیوست‌ها از [Delete Email Attachment](delete-email-attachment.md) یا [Delete Email Attachment By Name](delete-email-attachment-by-name.md) استفاده کنید. برای پیوست **یادداشت** (Note) استپ [Check Attachment](../note/check-attachment.md) را ببینید.

---

منبع: این استپ بر پایه‌ی پروژه‌ی متن‌باز CRM-Email-Workflow-Utilities (نوشته‌ی Jason Lattimer، مجوز MIT) است.

---

<!-- nav-bottom -->
[⬆ بازگشت به فهرست اصلی و همه‌ی استپ‌ها](../../README.md#steps)

</div>
