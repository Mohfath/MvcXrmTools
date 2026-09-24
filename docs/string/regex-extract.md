<div dir="rtl">

<!-- nav-top -->
[« صفحه‌ی اصلی](../../README.md) › [متن](../../README.md#string)

# استخراج بخشی از متن با Regular Expression در Dynamics CRM (استپ Regex Extract)

این استپ **اولین** بخشی از متن را که با یک Regular Expression هم‌خوانی دارد برمی‌گرداند.

برای استفاده از این استپ، در طراحی Workflow از منوی **Add Step** استپ **Regex Extract** را انتخاب کنید.

## پارامترهای ورودی

* **String To Search (اجباری)** : متنی که در آن جستجو می‌شود.
* **Pattern (اجباری)** : Regular Expression.

## پارامتر خروجی

* **Extracted String** : اولین بخش هم‌خوان، همان‌طور که در متن اصلی نوشته شده. اگر هم‌خوانی پیدا نشود، خالی است.

## مثال

| String To Search | Pattern | Extracted String |
|---|---|---|
| `Order #12345 shipped` | `\d+` | `12345` |
| `hello` | `H` | `h` |
| `abc` | `X` | خالی |

## نکته‌ها

* **بزرگی و کوچکی حروف همیشه نادیده گرفته می‌شود** (مثال دوم) و گزینه‌ای برای تغییر آن نیست.
* فقط اولین هم‌خوانی برگردانده می‌شود. برای فقط بررسی وجود الگو از [Regex Match](regex-match.md) و برای جایگزین کردن از [Regex Replace](regex-replace.md) استفاده کنید.
* اگر Pattern معتبر نباشد، استپ با خطا متوقف می‌شود.

---

<!-- nav-bottom -->
[⬆ بازگشت به فهرست اصلی و همه‌ی استپ‌ها](../../README.md#steps)

</div>
