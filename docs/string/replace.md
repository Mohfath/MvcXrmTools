<div dir="rtl">

<!-- nav-top -->
[« صفحه‌ی اصلی](../../README.md) › [متن](../../README.md#string)

# جایگزینی متن در Dynamics CRM (استپ Replace)

این استپ همه‌ی موارد یک متن را در متن دیگر با متن جدید جایگزین می‌کند.

برای استفاده از این استپ، در طراحی Workflow از منوی **Add Step** استپ **Replace** را انتخاب کنید.

## پارامترهای ورودی

* **String To Search (اجباری)** : متنی که جایگزینی روی آن انجام می‌شود.
* **Value To Replace (اجباری)** : متنی که باید جایگزین شود.
* **Replacement Value (اختیاری)** : متن جدید. اگر خالی باشد، Value To Replace از متن حذف می‌شود.

## پارامتر خروجی

* **Replaced String** : متن پس از جایگزینی.

## مثال

| String To Search | Value To Replace | Replacement Value | Replaced String |
|---|---|---|---|
| `Hello hello` | `hello` | `X` | `Hello X` |
| `a-b-c` | `-` | خالی | `abc` |

## نکته‌ها

* **بزرگی و کوچکی حروف مهم است** (مثال اول: `Hello` جایگزین نشد). اگر به بزرگی و کوچکی حروف کاری ندارید، از [Regex Replace](regex-replace.md) استفاده کنید که آن را نادیده می‌گیرد.
* برای جایگزین کردن با فاصله از [Replace With Space](replace-with-space.md) استفاده کنید.

---

<!-- nav-bottom -->
[⬆ بازگشت به فهرست اصلی و همه‌ی استپ‌ها](../../README.md#steps)

</div>
