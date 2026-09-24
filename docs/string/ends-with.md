<div dir="rtl">

<!-- nav-top -->
[« صفحه‌ی اصلی](../../README.md) › [متن](../../README.md#string)

# بررسی پایان یافتن متن با یک عبارت در Dynamics CRM (استپ Ends With)

این استپ بررسی می‌کند که یک متن با متن دیگری تمام می‌شود یا نه؛ مثلاً برای بررسی پسوند نام فایل.

برای استفاده از این استپ، در طراحی Workflow از منوی **Add Step** استپ **Ends With** را انتخاب کنید.

## پارامترهای ورودی

* **String To Search (اجباری)** : متنی که بررسی می‌شود.
* **Search For (اجباری)** : متنی که باید انتهای متن باشد.
* **Case Sensitive (اجباری، پیش‌فرض: False)** : اگر `True` باشد، بزرگی و کوچکی حروف مهم است. اگر `False` باشد، مهم نیست.

## پارامتر خروجی

* **Ends With String** : اگر متن با عبارت جستجو تمام شود `True` و در غیر این صورت `False` است.

## مثال

| String To Search | Search For | Case Sensitive | Ends With String |
|---|---|---|---|
| `Report.pdf` | `.PDF` | `False` | `True` |
| `Report.pdf` | `.PDF` | `True` | `False` |

## نکته‌ها

* برای بررسی شروع متن [Starts With](starts-with.md) و برای وجود عبارت در هر جای متن [Contains](contains.md) را استفاده کنید.

---

<!-- nav-bottom -->
[⬆ بازگشت به فهرست اصلی و همه‌ی استپ‌ها](../../README.md#steps)

</div>
