<div dir="rtl">

<!-- nav-top -->
[« صفحه‌ی اصلی](../../README.md) › [متن](../../README.md#string)

# رمزگشایی نویسه‌های HTML در Dynamics CRM (استپ Decode Html)

این استپ نویسه‌های کدگذاری‌شده‌ی HTML را به نویسه‌ی اصلی برمی‌گرداند (`&lt;` ← `<`، `&amp;` ← `&` و مانند آن).

برای استفاده از این استپ، در طراحی Workflow از منوی **Add Step** استپ **Decode Html** را انتخاب کنید.

## پارامترهای ورودی

* **String To Decode (اجباری)** : متنی که کدگذاری HTML دارد.

## پارامتر خروجی

* **Decoded String** : متن رمزگشایی‌شده.

## مثال

| String To Decode | Decoded String |
|---|---|
| `&lt;b&gt;Tom &amp; Jerry&lt;/b&gt;` | `<b>Tom & Jerry</b>` |

## نکته‌ها

* عکس این استپ [Encode Html](encode-html.md) است.
* خروجی [Remove Html](remove-html.md) نویسه‌های کدگذاری‌شده مثل `&amp;` را دست‌نخورده نگه می‌دارد؛ اگر لازم دارید، نتیجه را با Decode Html رمزگشایی کنید.

---

<!-- nav-bottom -->
[⬆ بازگشت به فهرست اصلی و همه‌ی استپ‌ها](../../README.md#steps)

</div>
