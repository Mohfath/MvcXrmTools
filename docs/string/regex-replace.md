<div dir="rtl">

<!-- nav-top -->
[« صفحه‌ی اصلی](../../README.md) › [متن](../../README.md#string)

# جایگزینی متن با Regular Expression در Dynamics CRM (استپ Regex Replace)

این استپ **همه‌ی** بخش‌های هم‌خوان با یک Regular Expression را با یک متن جایگزین می‌کند.

برای استفاده از این استپ، در طراحی Workflow از منوی **Add Step** استپ **Regex Replace** را انتخاب کنید.

## پارامترهای ورودی

* **String To Search (اجباری)** : متنی که جایگزینی روی آن انجام می‌شود.
* **Pattern (اجباری)** : Regular Expression.
* **Replacement Value (اختیاری)** : متن جایگزین. اگر خالی باشد، بخش‌های هم‌خوان حذف می‌شوند. می‌توانید از گروه‌های الگو با `$1` و `$2` و … استفاده کنید.

## پارامتر خروجی

* **Replaced String** : متن پس از جایگزینی.

## مثال

| String To Search | Pattern | Replacement Value | Replaced String |
|---|---|---|---|
| `a1b22c` | `\d+` | `#` | `a#b#c` |
| `John Smith` | `(\w+) (\w+)` | `$2, $1` | `Smith, John` |
| `a1b2` | `\d` | خالی | `ab` |

## نکته‌ها

* **بزرگی و کوچکی حروف همیشه نادیده گرفته می‌شود.**
* برای جایگزینی با فاصله از [Regex Replace With Space](regex-replace-with-space.md) و برای جایگزینی متن ساده (بدون Regular Expression) از [Replace](replace.md) استفاده کنید.

---

<!-- nav-bottom -->
[⬆ بازگشت به فهرست اصلی و همه‌ی استپ‌ها](../../README.md#steps)

</div>
