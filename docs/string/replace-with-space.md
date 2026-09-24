<div dir="rtl">

<!-- nav-top -->
[« صفحه‌ی اصلی](../../README.md) › [متن](../../README.md#string)

# جایگزینی متن با فاصله در Dynamics CRM (استپ Replace With Space)

این استپ همه‌ی موارد یک متن را با تعداد مشخصی فاصله (space) جایگزین می‌کند.

برای استفاده از این استپ، در طراحی Workflow از منوی **Add Step** استپ **Replace With Space** را انتخاب کنید.

## پارامترهای ورودی

* **String To Search (اجباری)** : متنی که جایگزینی روی آن انجام می‌شود.
* **Value To Replace (اجباری)** : متنی که باید جایگزین شود.
* **Number Of Spaces (اجباری)** : تعداد فاصله‌ای که جای هر مورد می‌آید.

## پارامتر خروجی

* **Replaced String** : متن پس از جایگزینی.

## مثال

| String To Search | Value To Replace | Number Of Spaces | Replaced String |
|---|---|---|---|
| `a-b` | `-` | `3` | `a   b` |

## نکته‌ها

* بزرگی و کوچکی حروف مهم است.
* برای جایگزین کردن با متن دلخواه از [Replace](replace.md) استفاده کنید.

---

<!-- nav-bottom -->
[⬆ بازگشت به فهرست اصلی و همه‌ی استپ‌ها](../../README.md#steps)

</div>
