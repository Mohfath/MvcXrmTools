<div dir="rtl">

<!-- nav-top -->
[« صفحه‌ی اصلی](../../README.md) › [متن](../../README.md#string)

# بررسی هم‌خوانی متن با Regular Expression در Dynamics CRM (استپ Regex Match)

این استپ بررسی می‌کند که یک متن با یک Regular Expression هم‌خوانی دارد یا نه.

برای استفاده از این استپ، در طراحی Workflow از منوی **Add Step** استپ **Regex Match** را انتخاب کنید.

## پارامترهای ورودی

* **String To Search (اجباری)** : متنی که بررسی می‌شود.
* **Pattern (اجباری)** : Regular Expression.

## پارامتر خروجی

* **Contains Pattern** : اگر بخشی از متن با الگو هم‌خوانی داشته باشد `True` و در غیر این صورت `False` است.

## مثال

| String To Search | Pattern | Contains Pattern |
|---|---|---|
| `abc123` | `^[a-z]+\d+$` | `True` |
| `ABC` | `abc` | `True` |

## نکته‌ها

* **بزرگی و کوچکی حروف همیشه نادیده گرفته می‌شود** (مثال دوم).
* هم‌خوانی در **هر جای** متن کافی است؛ برای بررسی کل متن الگو را با `^` و `$` بنویسید.
* برای گرفتن خودِ بخش هم‌خوان از [Regex Extract](regex-extract.md) استفاده کنید.

---

<!-- nav-bottom -->
[⬆ بازگشت به فهرست اصلی و همه‌ی استپ‌ها](../../README.md#steps)

</div>
