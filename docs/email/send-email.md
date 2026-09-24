<div dir="rtl">

<!-- nav-top -->
[« صفحه‌ی اصلی](../../README.md) › [ایمیل](../../README.md#email)

# ارسال ایمیل در Dynamics CRM (استپ Send Email)

این استپ یک ایمیل موجود (پیش‌نویس) را می‌فرستد. گیرنده‌ها، موضوع و متن باید از قبل روی ایمیل تنظیم شده باشند.

برای استفاده از این استپ، در طراحی Workflow از منوی **Add Step** استپ **Send Email** را انتخاب کنید.

## پارامترهای ورودی

* **Email To Send (اجباری)** : ایمیلی که فرستاده می‌شود (Lookup به ایمیل).

## پارامتر خروجی

* **Email Sent** : بعد از ارسال موفق برابر `True` است. اگر CRM درخواست ارسال را رد کند، استپ با خطا متوقف می‌شود و Workflow ادامه پیدا نمی‌کند.

## نکته‌ها

* ایمیل بلافاصله برای ارسال ثبت می‌شود؛ ارسال واقعی و تحویل به گیرنده به تنظیمات ایمیل CRM (Server-Side Sync، Email Router و مانند آن) بستگی دارد. **Email Sent = True** یعنی درخواست ارسال پذیرفته شد، نه اینکه ایمیل به دست گیرنده رسیده است.
* برای اینکه گیرنده‌ها را از یک تیم، نقش، واحد سازمانی، صف یا Connection اضافه کنید و همان‌جا ایمیل را بفرستید، پارامتر **Send Email?** استپ‌های [Email Team](email-team.md)، [Email Security Role](email-security-role.md)، [Email Business Unit](email-business-unit.md)، [Email Queue Members](email-queue-members.md) و [Email Connection](email-connection.md) را `True` کنید.
* استپ با دسترسی کاربر اجراکننده‌ی Workflow اجرا می‌شود.

---

منبع: این استپ بر پایه‌ی پروژه‌ی متن‌باز CRM-Email-Workflow-Utilities (نوشته‌ی Jason Lattimer، مجوز MIT) است.

---

<!-- nav-bottom -->
[⬆ بازگشت به فهرست اصلی و همه‌ی استپ‌ها](../../README.md#steps)

</div>
