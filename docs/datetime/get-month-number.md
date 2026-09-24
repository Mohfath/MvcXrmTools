<div dir="rtl">

<!-- nav-top -->
[« صفحه‌ی اصلی](../../README.md) › [تاریخ و زمان](../../README.md#datetime)

# گرفتن شماره‌ی ماه از تاریخ در Dynamics CRM (استپ Get Month Number)

این استپ شماره‌ی **ماه میلادی** یک تاریخ را برمی‌گرداند (`1` تا `12`).

برای استفاده از این استپ، در طراحی Workflow از منوی **Add Step** استپ **Get Month Number** را انتخاب کنید.

## پارامترهای ورودی

* **Date To Use (اجباری)** : تاریخ موردنظر.
* **Evaluate As User Local (اجباری، پیش‌فرض: True)** : اگر `True` باشد، تاریخ پیش از محاسبه با منطقه‌ی زمانی کاربر اجراکننده‌ی Workflow به وقت محلی تبدیل می‌شود؛ اگر `False`، همان UTC استفاده می‌شود.

## پارامتر خروجی

* **Month Number** : شماره‌ی ماه میلادی.

## مثال

| Date To Use | Month Number |
|---|---|
| 2026-09-24 | `9` |

## نکته‌ها

* ماه **میلادی** است. برای شماره‌ی ماه شمسی (مثلاً مهر = `7`) از [Get Date Persian Parts](../persian/get-date-persian-parts.md) استفاده کنید.
* برای نام ماه از [Get Month Name](get-month-name.md) استفاده کنید.

---

<!-- nav-bottom -->
[⬆ بازگشت به فهرست اصلی و همه‌ی استپ‌ها](../../README.md#steps)

</div>
