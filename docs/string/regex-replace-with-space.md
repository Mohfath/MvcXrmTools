<div dir="rtl">

<!-- nav-top -->
[« صفحه‌ی اصلی](../../README.md) › [متن](../../README.md#string)

# جایگزینی با فاصله با Regular Expression در Dynamics CRM (استپ Regex Replace With Space)

این استپ همه‌ی بخش‌های هم‌خوان با یک Regular Expression را با تعداد مشخصی فاصله (space) جایگزین می‌کند.

برای استفاده از این استپ، در طراحی Workflow از منوی **Add Step** استپ **Regex Replace With Space** را انتخاب کنید.

## پارامترهای ورودی

* **String To Search (اجباری)** : متنی که جایگزینی روی آن انجام می‌شود.
* **Number Of Spaces (اجباری)** : تعداد فاصله‌ای که جای هر بخش هم‌خوان می‌آید.
* **Pattern (اجباری)** : Regular Expression.

## پارامتر خروجی

* **Replaced String** : متن پس از جایگزینی.

## مثال

| String To Search | Pattern | Number Of Spaces | Replaced String |
|---|---|---|---|
| `a1b2` | `\d` | `2` | `a  b  ` |

## نکته‌ها

* **بزرگی و کوچکی حروف همیشه نادیده گرفته می‌شود.**
* برای جایگزین کردن با متن دلخواه از [Regex Replace](regex-replace.md) استفاده کنید.

---

<!-- nav-bottom -->
[⬆ بازگشت به فهرست اصلی و همه‌ی استپ‌ها](../../README.md#steps)

</div>
