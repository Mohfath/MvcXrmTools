<div dir="rtl">

<!-- nav-top -->
[« صفحه‌ی اصلی](../../README.md) › [تاریخ و زمان](../../README.md#datetime)

# گرفتن شماره‌ی فصل (سه‌ماهه) از تاریخ در Dynamics CRM (استپ Get Quarter Number Of Year)

این استپ شماره‌ی **فصل (سه‌ماهه) میلادی** یک تاریخ را برمی‌گرداند: ژانویه تا مارس `1`، آوریل تا ژوئن `2`، ژوئیه تا سپتامبر `3` و اکتبر تا دسامبر `4`.

برای استفاده از این استپ، در طراحی Workflow از منوی **Add Step** استپ **Get Quarter Number Of Year** را انتخاب کنید.

## پارامترهای ورودی

* **Date To Use (اجباری)** : تاریخ موردنظر.
* **Evaluate As User Local (اجباری، پیش‌فرض: True)** : اگر `True` باشد، تاریخ پیش از محاسبه با منطقه‌ی زمانی کاربر اجراکننده‌ی Workflow به وقت محلی تبدیل می‌شود؛ اگر `False`، همان UTC استفاده می‌شود.

## پارامتر خروجی

* **Quarter Number Of Year** : شماره‌ی فصل از `1` تا `4`.

## مثال

| Date To Use | Quarter Number Of Year |
|---|---|
| 2026-09-24 | `3` |

## نکته‌ها

* فصل **میلادی** است؛ با فصل شمسی (بهار از اول فروردین) یکی نیست. برای فصل شمسی از [Get Date Persian Parts](../persian/get-date-persian-parts.md) (خروجی «شماره فصل») استفاده کنید.
* برای اولین و آخرین روز فصل از [Get Quarter Start End](get-quarter-start-end.md) استفاده کنید.

---

<!-- nav-bottom -->
[⬆ بازگشت به فهرست اصلی و همه‌ی استپ‌ها](../../README.md#steps)

</div>
