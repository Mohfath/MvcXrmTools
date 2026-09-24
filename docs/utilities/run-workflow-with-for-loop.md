<div dir="rtl">

<!-- nav-top -->
[« صفحه‌ی اصلی](../../README.md) › [ابزارهای عمومی](../../README.md#utilities)

# اجرای Workflow چند بار روی یک رکورد در Dynamics CRM (استپ Run Workflow With For Loop)

این استپ یک Workflow دیگر را **به تعداد مشخص** روی یک رکورد اجرا می‌کند (مثل یک حلقه‌ی `for`).

برای استفاده از این استپ، در طراحی Workflow از منوی **Add Step** به گروه **MvcTeam Utilities** بروید و **Run Workflow With For Loop** را انتخاب کنید.

## پارامترهای ورودی

* **Record ID (اجباری)** : شناسه‌ی (GUID) رکوردی که Workflow روی آن اجرا می‌شود، به‌صورت متن. برای گرفتن آن از آدرس داینامیک می‌توانید از [Get Record Id](get-record-id.md) استفاده کنید.
* **# of Repeats (اجباری)** : تعداد دفعات اجرا، از `1` تا `100`.
* **Process (اجباری در عمل)** : Workflow (فرایند) که باید اجرا شود (Lookup به Workflow).

این استپ پارامتر خروجی ندارد.

## نحوه‌ی کار

Workflow انتخاب‌شده را به تعداد `# of Repeats` بار، یکی پس از دیگری، روی رکورد اجرا می‌کند (هر بار درخواست اجرای Workflow به CRM می‌رود).

## خطاها

| پیام | علت |
|---|---|
| `Process is required.` | Workflow انتخاب نشده است. |
| `Record ID '…' is not a valid GUID.` | Record ID یک GUID معتبر نیست. |
| `# of Repeats must be between 1 and 100; got …` | تعداد تکرار بیرون از بازه‌ی `1` تا `100` است. |

## نکته‌ها

* Workflow انتخاب‌شده باید برای **همان نوع رکوردی** ساخته شده باشد که Record ID به آن تعلق دارد و باید به‌صورت **On-demand** قابل اجرا باشد.
* استپ با دسترسی کاربر اجراکننده‌ی Workflow اجرا می‌شود.
* مراقب حلقه‌ی بی‌نهایت باشید: اگر Workflow انتخاب‌شده دوباره همین استپ را صدا بزند، اجراها تکرار می‌شوند.
* برای اجرای یک Workflow روی **چند رکورد مختلف** از [Query Run Workflow On Results](query-run-workflow-on-results.md) استفاده کنید.

---

<!-- nav-bottom -->
[⬆ بازگشت به فهرست اصلی و همه‌ی استپ‌ها](../../README.md#steps)

</div>
