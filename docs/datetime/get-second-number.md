<div dir="rtl">

<!-- nav-top -->
[« صفحه‌ی اصلی](../../README.md) › [تاریخ و زمان](../../README.md#datetime)

# گرفتن ثانیه از تاریخ در Dynamics CRM (استپ Get Second Number)

این استپ ثانیه (`0` تا `59`) یک تاریخ و زمان را برمی‌گرداند.

برای استفاده از این استپ، در طراحی Workflow از منوی **Add Step** استپ **Get Second Number** را انتخاب کنید.

## پارامترهای ورودی

* **Date To Use (اجباری)** : تاریخ و زمان موردنظر.
* **Evaluate As User Local (اجباری، پیش‌فرض: True)** : اگر `True` باشد، زمان ورودی (UTC) پیش از محاسبه با منطقه‌ی زمانی کاربر اجراکننده‌ی Workflow به وقت محلی تبدیل می‌شود؛ اگر `False`، همان UTC استفاده می‌شود. (منطقه‌ی زمانی معمولاً ثانیه را عوض نمی‌کند.)

## پارامتر خروجی

* **Second Number** : ثانیه از `0` تا `59`.

## مثال

| Date To Use (UTC) | Second Number |
|---|---|
| 2026-09-24 08:30:15 | `15` |

## نکته‌ها

* مشابه: [Get Hour Number](get-hour-number.md) و [Get Minute Number](get-minute-number.md).

---

<!-- nav-bottom -->
[⬆ بازگشت به فهرست اصلی و همه‌ی استپ‌ها](../../README.md#steps)

</div>
