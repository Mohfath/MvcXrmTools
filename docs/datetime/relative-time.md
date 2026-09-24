<div dir="rtl">

<!-- nav-top -->
[« صفحه‌ی اصلی](../../README.md) › [تاریخ و زمان](../../README.md#datetime)

# نمایش زمان نسبی مثل «۳ ساعت پیش» در Dynamics CRM (استپ Relative Time)

این استپ فاصله‌ی دو تاریخ را به‌صورت یک عبارت انگلیسی مثل `3 hours ago` یا `yesterday` برمی‌گرداند.

برای استفاده از این استپ، در طراحی Workflow از منوی **Add Step** استپ **Relative Time** را انتخاب کنید.

## پارامترهای ورودی

* **Starting Date (اجباری)** : تاریخ گذشته (مثلاً زمان ایجاد رکورد).
* **Ending Date (اجباری)** : تاریخ مبنا (معمولاً «اکنون»).

## پارامتر خروجی

* **Relative Time String** : عبارت انگلیسیِ فاصله‌ی `Ending Date − Starting Date`.

## جدول خروجی‌ها

فاصله‌ی `Ending Date − Starting Date` (با Starting Date برابر 2026-09-24 08:00):

| فاصله | Relative Time String |
|---|---|
| ۱ ثانیه | `one second ago` |
| ۳۰ ثانیه | `30 seconds ago` |
| ۹۰ ثانیه | `a minute ago` |
| ۵ دقیقه | `5 minutes ago` |
| ۴۴ دقیقه | `44 minutes ago` |
| ۴۵ دقیقه تا ۸۹ دقیقه (مثلاً ۵۰ دقیقه) | `an hour ago` |
| ۳ ساعت | `3 hours ago` |
| ۲۳ ساعت | `23 hours ago` |
| ۳۰ ساعت | `yesterday` |
| ۵ روز | `5 days ago` |
| ۲۹ روز | `29 days ago` |
| ۴۵ روز | `one month ago` |
| ۶۰ روز | `2 months ago` |
| ۲۰۰ روز | `6 months ago` |
| ۳۶۴ روز و ۴۰۰ روز | `one year ago` |
| ۸۰۰ روز | `2 years ago` |
| صفر | `0 seconds ago` |
| Ending Date قبل از Starting Date | `in the future` |

## نکته‌ها

* خروجی **همیشه انگلیسی** است و برای نمایش به کاربر فارسی‌زبان مناسب نیست. برای متن فارسی از عدد اختلاف ([Date Diff Days](date-diff-days.md) و مشابه) و ساختن جمله‌ی فارسی با [Join](../string/join.md) استفاده کنید.
* اگر Starting Date بعد از Ending Date باشد، نتیجه `in the future` است (Starting Date را زمان گذشته بگذارید).
* ماه در این استپ ۳۰ روز و سال ۳۶۵ روز حساب می‌شود.

---

<!-- nav-bottom -->
[⬆ بازگشت به فهرست اصلی و همه‌ی استپ‌ها](../../README.md#steps)

</div>
