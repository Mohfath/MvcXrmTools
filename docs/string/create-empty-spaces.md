<div dir="rtl">

<!-- nav-top -->
[« صفحه‌ی اصلی](../../README.md) › [متن](../../README.md#string)

# ساخت متن فاصله (space) در Dynamics CRM (استپ Create Empty Spaces)

این استپ متنی می‌سازد که فقط از تعداد مشخصی فاصله (space) تشکیل شده است؛ برای ساخت فاصله بین دو بخش از متن.

برای استفاده از این استپ، در طراحی Workflow از منوی **Add Step** استپ **Create Empty Spaces** را انتخاب کنید.

## پارامترهای ورودی

* **Number Of Spaces (اجباری)** : تعداد فاصله‌ها.

## پارامتر خروجی

* **Empty String** : متنی که فقط فاصله دارد.

## مثال

| Number Of Spaces | Empty String |
|---|---|
| `3` | سه فاصله (`"   "`) |

## نکته‌ها

* عدد منفی خطا می‌دهد (`'count' must be non-negative`).
* برای ساختن متنِ بین دو رشته می‌توانید نتیجه را با [Join](join.md) به کار ببرید.

---

<!-- nav-bottom -->
[⬆ بازگشت به فهرست اصلی و همه‌ی استپ‌ها](../../README.md#steps)

</div>
