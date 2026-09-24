<div dir="rtl">

<!-- nav-top -->
[« صفحه‌ی اصلی](../../README.md) › [متن](../../README.md#string)

# بررسی شامل بودن متن در Dynamics CRM (استپ Contains)

این استپ بررسی می‌کند که یک متن شامل متن دیگری هست یا نه.

برای استفاده از این استپ، در طراحی Workflow از منوی **Add Step** استپ **Contains** را انتخاب کنید.

## پارامترهای ورودی

* **String To Search (اجباری)** : متنی که در آن جستجو می‌شود.
* **Search For (اجباری)** : متنی که جستجو می‌شود.
* **Case Sensitive (اجباری، پیش‌فرض: False)** : اگر `True` باشد، بزرگی و کوچکی حروف مهم است. اگر `False` باشد، مهم نیست.

## پارامتر خروجی

* **Contains String** : اگر متن پیدا شود `True` و در غیر این صورت `False` است.

## مثال

| String To Search | Search For | Case Sensitive | Contains String |
|---|---|---|---|
| `Hello World` | `world` | `False` | `True` |
| `Hello World` | `world` | `True` | `False` |

## نکته‌ها

* بررسی شروع و پایان متن با [Starts With](starts-with.md) و [Ends With](ends-with.md) انجام می‌شود.

---

<!-- nav-bottom -->
[⬆ بازگشت به فهرست اصلی و همه‌ی استپ‌ها](../../README.md#steps)

</div>
