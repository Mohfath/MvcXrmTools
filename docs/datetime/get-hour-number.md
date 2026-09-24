<div dir="rtl">

<!-- nav-top -->
[« صفحه‌ی اصلی](../../README.md) › [تاریخ و زمان](../../README.md#datetime)

# گرفتن ساعت از تاریخ در Dynamics CRM (استپ Get Hour Number)

این استپ ساعت (۲۴ ساعته، `0` تا `23`) یک تاریخ و زمان را برمی‌گرداند.

برای استفاده از این استپ، در طراحی Workflow از منوی **Add Step** استپ **Get Hour Number** را انتخاب کنید.

## پارامترهای ورودی

* **Date To Use (اجباری)** : تاریخ و زمان موردنظر.
* **Evaluate As User Local (اجباری، پیش‌فرض: True)** : اگر `True` باشد، زمان ورودی (UTC) پیش از محاسبه با منطقه‌ی زمانی کاربر اجراکننده‌ی Workflow به وقت محلی تبدیل می‌شود؛ اگر `False`، همان UTC استفاده می‌شود.

## پارامتر خروجی

* **Hour Number** : ساعت از `0` تا `23`.

## مثال

با منطقه‌ی زمانی کاربر برابر UTC+3:30 (ایران):

| Date To Use (UTC) | Evaluate As User Local | Hour Number |
|---|---|---|
| 2026-09-22 21:00 | `False` | `21` |
| 2026-09-22 21:00 | `True` | `0` (ساعت ۰۰:۳۰ به وقت کاربر) |

## نکته‌ها

* برای شرط‌هایی مثل «فقط در ساعات اداری» از این استپ با `Evaluate As User Local = True` استفاده کنید تا ساعت با ساعت دیواریِ کاربر یکی باشد.
* مشابه: [Get Minute Number](get-minute-number.md) و [Get Second Number](get-second-number.md).

---

<!-- nav-bottom -->
[⬆ بازگشت به فهرست اصلی و همه‌ی استپ‌ها](../../README.md#steps)

</div>
