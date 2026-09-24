<div dir="rtl">

<!-- nav-top -->
[« صفحه‌ی اصلی](../../README.md) › [ابزارهای عمومی](../../README.md#utilities)

# اجرای Workflow روی همه‌ی نتایج یک جستجو در Dynamics CRM (استپ Query Run Workflow On Results)

این استپ یک Workflow را روی **هر رکوردی** اجرا می‌کند که یک View یا یک FetchXml برمی‌گرداند. برای عملیات دسته‌ای مفید است: مثلاً «برای همه‌ی فرصت‌های فروشِ بازِ این مشتری، Workflow تأیید را اجرا کن».

برای استفاده از این استپ، در طراحی Workflow از منوی **Add Step** به گروه **MvcTeam Utilities** بروید و **Query Run Workflow On Results** را انتخاب کنید.

## پارامترهای ورودی

* **Pick a System View to Use** : یک View سیستمی (Lookup به System View).
* **or pick a Personal View to Use** : یک View شخصی (Lookup به Personal View).
* **or enter FetchXML to Use** : متن FetchXml.
* **Workflow to Run (اجباری)** : Workflow (فرایند) که روی هر رکورد اجرا می‌شود (Lookup به Workflow).

**فقط یکی از سه روش تعریف جستجو کافی است.** اگر بیش از یکی پر شود، اولویت با View شخصی، بعد View سیستمی و بعد FetchXml است. اگر هیچ‌کدام پر نباشد استپ با خطای `You need to pick a System View, a Personal View, or specify FetchXML for the query.` متوقف می‌شود.

## پارامتر خروجی

* **Number of Workflows Started** : تعداد Workflowهایی که شروع شدند.

## نحوه‌ی کار

1. جستجو اجرا می‌شود (همه‌ی صفحه‌های نتیجه خوانده می‌شود).
2. Workflow انتخاب‌شده روی رکوردها **یکی پس از دیگری** شروع می‌شود و در **اولین خطا متوقف** می‌شود.
3. تعداد Workflowهای شروع‌شده برگردانده می‌شود.

* اگر در FetchXml یا View، فیلتری «شناسه‌ی رکورد خالی نباشد» روی یک Link به موجودیت **رکورد جاری Workflow** باشد، جستجو به همان رکورد جاری محدود می‌شود (روش معمول برای گرفتن «رکوردهای مرتبط با رکورد فعلی»).

## خطاها و محدودیت‌ها

* **موجودیت Workflow باید با موجودیت جستجو یکی باشد.** اگر نباشد، استپ با پیام `The workflow's entity (…) does not match the query's entity (…).` متوقف می‌شود.
* جستجوهای **تجمیعی و گروه‌بندی‌شده** (Aggregate) برای این استپ پشتیبانی نمی‌شوند، چون هر ردیف آن‌ها رکورد مشخصی ندارد.
* استپ با دسترسی کاربر اجراکننده‌ی Workflow اجرا می‌شود؛ پس فقط رکوردهایی را پیدا می‌کند و Workflow را روی رکوردهایی شروع می‌کند که آن کاربر اجازه‌اش را دارد.

## نکته‌ها

* برای اجرای یک Workflow چند بار روی **یک** رکورد از [Run Workflow With For Loop](run-workflow-with-for-loop.md) استفاده کنید.
* برای گرفتن نتیجه‌ی جستجو به‌صورت جدول یا لیست به‌جای اجرای Workflow از [Query Get Results](query-get-results.md) استفاده کنید.

---

منبع: این استپ بر پایه‌ی استپ QueryRunWorkflowOnResults از پروژه‌ی متن‌باز [WorkflowElements](https://github.com/akaskela/WorkflowElements) (نوشته‌ی Aiden Kaskela، مجوز MIT) است.

---

<!-- nav-bottom -->
[⬆ بازگشت به فهرست اصلی و همه‌ی استپ‌ها](../../README.md#steps)

</div>
