<div dir="rtl">

<!-- nav-top -->
[« صفحه‌ی اصلی](../../README.md) › [متن](../../README.md#string)

# بررسی شروع شدن متن با یک عبارت در Dynamics CRM (استپ Starts With)

این استپ بررسی می‌کند که یک متن با متن دیگری شروع می‌شود یا نه.

برای استفاده از این استپ، در طراحی Workflow از منوی **Add Step** استپ **Starts With** را انتخاب کنید.

## پارامترهای ورودی

* **String To Search (اجباری)** : متنی که بررسی می‌شود.
* **Search For (اجباری)** : متنی که باید ابتدای متن باشد.
* **Case Sensitive (اجباری، پیش‌فرض: False)** : اگر `True` باشد، بزرگی و کوچکی حروف مهم است. اگر `False` باشد، مهم نیست.

## پارامتر خروجی

* **Starts With String** : اگر متن با عبارت جستجو شروع شود `True` و در غیر این صورت `False` است.

## مثال

| String To Search | Search For | Case Sensitive | Starts With String |
|---|---|---|---|
| `Report.pdf` | `report` | `False` | `True` |
| `Report.pdf` | `report` | `True` | `False` |

## نکته‌ها

* برای بررسی پایان متن [Ends With](ends-with.md) و برای وجود عبارت در هر جای متن [Contains](contains.md) را استفاده کنید.

---

<!-- nav-bottom -->
[⬆ بازگشت به فهرست اصلی و همه‌ی استپ‌ها](../../README.md#steps)

</div>
