<div dir="rtl">

<!-- nav-top -->
[« صفحه‌ی اصلی](../../README.md) › [تاریخ و زمان](../../README.md#datetime)

# افزودن روز به تاریخ در Dynamics CRM (استپ Add Days)

این استپ تعداد مشخصی روز به یک تاریخ اضافه می‌کند (یا با عدد منفی از آن کم می‌کند). ساعت، دقیقه و ثانیه‌ی تاریخ بدون تغییر می‌ماند.

برای استفاده از این استپ، در طراحی Workflow از منوی **Add Step** استپ **Add Days** را انتخاب کنید.

## پارامترهای ورودی

* **Original Date (اجباری)** : تاریخ اولیه.
* **Days To Add (اجباری)** : تعداد روز. عدد منفی به عقب برمی‌گردد.

## پارامتر خروجی

* **Updated Date** : تاریخ جدید.

## مثال

| Original Date (UTC) | Days To Add | Updated Date |
|---|---|---|
| 2026-09-24 08:30:15 | `10` | 2026-10-04 08:30:15 |
| 2026-09-24 08:30:15 | `-5` | 2026-09-19 08:30:15 |

## نکته‌ها

* محاسبه روی همان تاریخی که می‌دهید انجام می‌شود؛ CRM تاریخ‌ها را به‌صورت UTC می‌دهد، پس نتیجه هم UTC است.
* اگر می‌خواهید روزهای تعطیل را نشمارید، از [Add Business Days](add-business-days.md) استفاده کنید.
* برای نمایش نتیجه به تاریخ شمسی از [Format Persian DateTime](../persian/format-persian-date-time.md) استفاده کنید.
* استپ‌های مشابه: [Add Weeks](add-weeks.md)، [Add Months](add-months.md)، [Add Years](add-years.md)، [Add Hours](add-hours.md) و [Add Minutes](add-minutes.md).

---

<!-- nav-bottom -->
[⬆ بازگشت به فهرست اصلی و همه‌ی استپ‌ها](../../README.md#steps)

</div>
