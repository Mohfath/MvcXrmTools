<div dir="rtl">

<!-- nav-top -->
[« صفحه‌ی اصلی](../../README.md) › [متن](../../README.md#string)

# رمزگشایی متن آدرس اینترنتی (URL) در Dynamics CRM (استپ Url Decode)

این استپ متنی را که برای آدرس اینترنتی کدگذاری شده (نویسه‌هایی مثل `%20` و `%26`) به متن اصلی برمی‌گرداند.

برای استفاده از این استپ، در طراحی Workflow از منوی **Add Step** استپ **Url Decode** را انتخاب کنید.

## پارامترهای ورودی

* **String To Decode (اجباری)** : متن کدگذاری‌شده.

## پارامتر خروجی

* **Decoded String** : متن اصلی.

## مثال

| String To Decode | Decoded String |
|---|---|
| `a%20b%26c%3Dd%2F%C3%A9` | `a b&c=d/é` |

## نکته‌ها

* عکس این استپ [Url Encode](url-encode.md) است. برای رمزگشایی HTML از [Decode Html](decode-html.md) استفاده کنید.

---

<!-- nav-bottom -->
[⬆ بازگشت به فهرست اصلی و همه‌ی استپ‌ها](../../README.md#steps)

</div>
