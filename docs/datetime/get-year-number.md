<div dir="rtl">

<!-- nav-top -->
[« صفحه‌ی اصلی](../../README.md) › [تاریخ و زمان](../../README.md#datetime)

# گرفتن سال از تاریخ در Dynamics CRM (استپ Get Year Number)

این استپ شماره‌ی **سال میلادی** یک تاریخ را برمی‌گرداند.

برای استفاده از این استپ، در طراحی Workflow از منوی **Add Step** استپ **Get Year Number** را انتخاب کنید.

## پارامترهای ورودی

* **Date To Use (اجباری)** : تاریخ موردنظر.
* **Evaluate As User Local (اجباری، پیش‌فرض: True)** : اگر `True` باشد، تاریخ پیش از محاسبه با منطقه‌ی زمانی کاربر اجراکننده‌ی Workflow به وقت محلی تبدیل می‌شود؛ اگر `False`، همان UTC استفاده می‌شود.

## پارامتر خروجی

* **Year Number** : سال میلادی.

## مثال

| Date To Use | Year Number |
|---|---|
| 2026-09-24 | `2026` |

## نکته‌ها

* سال **میلادی** است. برای سال شمسی (مثلاً `1405`) از [Get Date Persian Parts](../persian/get-date-persian-parts.md) استفاده کنید.

---

<!-- nav-bottom -->
[⬆ بازگشت به فهرست اصلی و همه‌ی استپ‌ها](../../README.md#steps)

</div>
