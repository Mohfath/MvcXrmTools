<div dir="rtl">

<!-- nav-top -->
[« صفحه‌ی اصلی](../../README.md) › [تاریخ و زمان](../../README.md#datetime)

# گرفتن نام روز هفته از تاریخ در Dynamics CRM (استپ Get Day Of Week)

این استپ نام روز هفته را به **انگلیسی** برمی‌گرداند: `Sunday`، `Monday` … `Saturday`.

برای استفاده از این استپ، در طراحی Workflow از منوی **Add Step** استپ **Get Day Of Week** را انتخاب کنید.

## پارامترهای ورودی

* **Date To Use (اجباری)** : تاریخ موردنظر.
* **Evaluate As User Local (اجباری، پیش‌فرض: True)** : اگر `True` باشد، تاریخ پیش از محاسبه با منطقه‌ی زمانی کاربر اجراکننده‌ی Workflow به وقت محلی تبدیل می‌شود؛ اگر `False`، همان UTC استفاده می‌شود.

## پارامتر خروجی

* **Day Of Week** : نام انگلیسی روز هفته.

## مثال

| Date To Use | Day Of Week |
|---|---|
| 2026-09-20 | `Sunday` |
| 2026-09-21 | `Monday` |
| 2026-09-24 | `Thursday` |
| 2026-09-25 | `Friday` |
| 2026-09-26 | `Saturday` |

## نکته‌ها

* خروجی همیشه انگلیسی است. برای نام **فارسی** روز (مثلاً «پنج شنبه») از [Format Persian DateTime](../persian/format-persian-date-time.md) با `dddd` استفاده کنید.
* برای شماره‌ی روز از [Get Day Number Of Week](get-day-number-of-week.md) استفاده کنید.

---

<!-- nav-bottom -->
[⬆ بازگشت به فهرست اصلی و همه‌ی استپ‌ها](../../README.md#steps)

</div>
