<div dir="rtl">

<!-- nav-top -->
[« صفحه‌ی اصلی](../../README.md) › [تاریخ و زمان](../../README.md#datetime)

# گرفتن دقیقه از تاریخ در Dynamics CRM (استپ Get Minute Number)

این استپ دقیقه (`0` تا `59`) یک تاریخ و زمان را برمی‌گرداند.

برای استفاده از این استپ، در طراحی Workflow از منوی **Add Step** استپ **Get Minute Number** را انتخاب کنید.

## پارامترهای ورودی

* **Date To Use (اجباری)** : تاریخ و زمان موردنظر.
* **Evaluate As User Local (اجباری، پیش‌فرض: True)** : اگر `True` باشد، زمان ورودی (UTC) پیش از محاسبه با منطقه‌ی زمانی کاربر اجراکننده‌ی Workflow به وقت محلی تبدیل می‌شود؛ اگر `False`، همان UTC استفاده می‌شود.

## پارامتر خروجی

* **Minute Number** : دقیقه از `0` تا `59`.

## مثال

| Date To Use (UTC) | Evaluate As User Local | Minute Number |
|---|---|---|
| 2026-09-24 08:30:15 | `False` | `30` |

## نکته‌ها

* برای اختلاف ساعت‌های نیم‌ساعته (مثل ایران) توجه کنید: با `Evaluate As User Local = True` و منطقه‌ی زمانی UTC+3:30، ساعت ۰۸:۰۰ UTC می‌شود ۱۱:۳۰ و دقیقه `30` است.
* مشابه: [Get Hour Number](get-hour-number.md) و [Get Second Number](get-second-number.md).

---

<!-- nav-bottom -->
[⬆ بازگشت به فهرست اصلی و همه‌ی استپ‌ها](../../README.md#steps)

</div>
