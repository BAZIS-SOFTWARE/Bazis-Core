# Проверка диаграмм и документации

Дата ревизии: 2 октября 2026 года. Проверены 121 активная диаграмма, 477 типов и 2361 локальных ссылок в активных Markdown-документах.

## Рендер

Использован сервер [192.168.11.202:8080](http://192.168.11.202:8080), указанный в [инструкциях архитектора](../.codex/agents/core-architect.md). Исходники отправлены в PlantUML Server, ответы SVG разобраны, проверены сообщения об ошибках и фактические размеры. Рендер соответствует SHA-256 актуальных исходников диаграмм.

Ошибок рендера нет. Все схемы помещаются в принятую для этой ревизии область 1800 × 1600 пикселей при шрифте Arial 12; уменьшение масштаба для соблюдения границ не использовалось. Максимальные размеры по двум осям: 1448 × 1584 (могут относиться к разным схемам). Это проверка размеров серверного SVG, а не фактического размера панели конкретного клиента.

Контрольные PNG `ProjectEvents`, `OperationalController`, `Model-3` и `Project-2` дополнительно просмотрены: текст читается, содержимое не обрезано. Исторические схемы с пометкой «АРХИВ» и прежние PNG не входят в проверенный набор.

## Полнота и связи

| Проект | Типов |
| --- | --- |
| Geometry | 25 |
| GmshApi | 23 |
| MaterialDB | 23 |
| Model | 99 |
| OperationalController | 126 |
| PostProc | 30 |
| PreProc | 6 |
| Project | 119 |
| ResultDB | 11 |
| SurfaceMesher | 15 |

Все перечисленные типы представлены на подробных схемах. Объявленные члены интерфейсов сверены с исходниками, кроме удаляемых событий нижних библиотек и вида. Проверены локальные ссылки, существование именованных поддиаграмм, уникальность псевдонимов и принадлежность события контракту `IProjectEvents`.

Внутренний протокол передачи результата `ModelView` остаётся предметом отдельного проектирования. Диаграммы не подтверждают выполнение миграции C# и не заменяют проверку поведения после неё. При первоначальном обновлении документации прежние правки C# были сохранены без изменений. Последующий откат делегатов `Append` изменяет реализацию и покрыт отдельной проверкой ниже.

## Связность чтения Model

После проверки навигации обновлены `Model.mdpuml`, все 20 поддиаграмм и соседние Markdown-описания. Основная схема указывает на каждую поддиаграмму, а каждая поддиаграмма содержит обратный переход к основной. Из любой из 21 схемы можно достигнуть всех остальных через эти переходы.

Для всех 46 вхождений `reference` проверены видимые адреса подробностей и соответствие ссылки диаграмме, действительно раскрывающей тип. Для `IObjectsData` доступны обе части — `Model-14` и `Model-15`. Типы `Geometry` имеют относительные адреса схем соответствующей библиотеки. Легенды и Markdown-описания содержат переходы по базовым типам и типам в полных сигнатурах, включая `IModelData.ObjectData`.

Проверены 205 локальных ссылок из диаграмм на схемы и описания API: целевые файлы существуют, ссылки присутствуют в серверных SVG. Интерактивность переходов зависит от поддержки просмотрщиком относительных ссылок; видимые названия позволяют найти подробности и в статическом изображении. При исправлении навигации состав членов типов был сохранён. После отката делегатов `Append` обновлены контракты в `Model-3` и `Model-16`; состав членов остальных схем `Model` тогда был сохранён. При исправлении массовых операций также обновлены `Model-18` и `Model-20`.

Все 21 диаграмма `Model` прошли повторный рендер без ошибок. Максимальная ширина — 769 пикселей, максимальная высота — 1584 пикселя; уменьшение масштаба не применялось. Контрольный PNG `Model-3` просмотрен после изменения: адреса типов и легенда читаются, содержимое не обрезано. Проверка достижимости между схемами в этой ревизии выполнена для `Model`.

## Откат делегатов Append

Удалены перегрузки с `Action` из `ObjectsSet.Append`, `ObjectsCollection.Append` и `ModelData.Append`, их интерфейсов и вызовов контроллера. В контроллере сообщения о добавлении формируются после завершённого `Append`, по одному на набор или добавленную группу; ошибка нумерации передаётся исключением. Изменения данных до ошибки автоматически не откатываются.

Состав интерфейсов сверен с текущими C#-исходниками через Roslyn, исключая события нижних библиотек, которые ещё предстоит удалить. Диаграммы `Model-3`, `Model-16` и `OperationalController-3` обновлены и повторно отрендерены. Все 121 активная схема помещаются в принятую область; навигация всех 21 схемы `Model` проверена после удаления перегрузок.

Сборка `TestProject` успешна. Все 34 теста выбранных классов `ProjectControllerContractTests` и `PointDataChangeTests` прошли, включая пустое добавление, 1 и 128 объектов, заполненные и пустые наборы, совпадающие имена в разных коллекциях, завершённое слияние и ошибку нумерации без уведомлений из незавершённого добавления.

Расширенный запуск `TestModelCore` завершился результатом 61 успешный тест и 6 ошибок: три проверки возвращаемого имени набора (`AppendEmptySetToFilledSet`, `AppendFilledSetToEmptySet`, `AppendEmptySetToEmptySet`), `CreatePoint_ShouldThrow_WhenGeometryIsNotInitialized` и два набора данных `MergeNodes`. Возвращаемое значение `ObjectsSet.Append` совпадает с исходным поведением до введения делегата: возвращается имя источника, тогда как эти три теста ожидают имя приёмника. Алгоритмы создания геометрии и слияния узлов данным откатом не изменялись. Устаревший `TestModel` не запускался.

## Исправление уведомлений массовых операций

Сигнатуры `ObjectsSet.Renumber`, `MarkSurfaces.Mark` и `IObjectsMerger.Merge` сверены с текущими исходниками Roslyn: делегатов уведомления нет, операции возвращают сведения об изменениях. Обновлены `Model-18`, `Model-20`, `OperationalController-3`, `OperationalController-23`, обзор контроллера и схема сообщений. Все 121 активная диаграмма повторно отрендерены без ошибок и превышения размеров. Связность всех 21 схемы `Model`, адреса всех 46 ссылочных типов и ссылки в SVG повторно проверены. Контрольный PNG `ProjectEvents` просмотрен: текст читается и не обрезан.

Сборки `TestProject` и действующего `TestModelCore` успешны. Выбранные 66 тестов `BulkDataChangeTests`, `GeometryBatchChangeTests`, `MarkSurfacesTests`, `PointDataChangeTests` и `ProjectControllerContractTests` прошли: координаты, порядок элементов, нумерация, удаление до 4096 узлов, связанные наборы, повторная команда без изменений, создание границы, уникальные изменённые владельцы граней и слияние узлов. С настоящей `gmsh.dll` проверены генерация сетки и рекурсивное удаление геометрии. Обработчики проверяют завершённое состояние данных в момент уведомления и число сообщений по наборам. Устаревший `TestModel` не запускался.

Незавершённая миграция событий IO и `ModelView` остаётся в [реестре расхождений](ArchitectureDifferences.md). Предыдущие шесть ошибок расширенного запуска `TestModelCore` выше относятся к предыдущей проверке; этот запуск их повторно не проверял.

## Демонтаж LoadEvent: проверка 3 октября 2026 года

Из активных загрузчиков сетки, проекта и данных задачи удалены `LoadEvent`, вызовы и подписки. Удалены `Model.Events.LoaderEventArgs`, `Model.Interfaces.ILoaderEventArgs`, `Project.IO.LoadEventArgs` и `Project.Tasks.IO.LoadEventArgs`. `Project.Interfaces.IO.ILoadEventArgs` сохранён только как исходник для архивных `Project/Results` и исключён из `Project.csproj`. Старый `Model.Interfaces.ILoader` сохраняет `Load(path)` без события. Числовой прогресс в `IModelLoader`, `IProjectLoader` и `ITaskDataLoader` передаётся через необязательный `IProgress<int>`.

Сборки `TestProject/TestProject.csproj` и `TestModelCore/TestModeCore.csproj` с `--no-restore` и `-p:GeneratePackageOnBuild=false` завершились без ошибок. Устаревший `TestModel` не собирался. Поиск подтверждает отсутствие демонтируемых событий и типов аргументов в активном коде; события загрузки результатов и сохранения не входят в выполненный объём. Проверка пробелов в изменённых файлах прошла.

Запущен весь класс `ProjectControllerContractTests` (19 кейсов) и новые `LoaderProgressTests` (12 кейсов): **31/31 успешно, без пропусков**. [TRX результатов](../tmp/load-event-verification/load-event-contracts.trx). Новые тесты используют непустые временные файлы `bpf`, `bpf2`, `BPF2` и вызовы через интерфейсы загрузчиков проекта и данных задачи. Проверены данные сетки и условий, сохранение ссылок на группы и узлы, пропуск строк до раздела «Проект», вызовы с опущенным параметром, явным `null` и получателем прогресса. Прогресс начинается с 0%, достигает 100%, не убывает и содержит промежуточные значения. Существующие проверки также подтверждают итоговый `Loaded`, поведение при ошибке чтения, текстовый прогресс и пустой бинарный STL.

Команда проверки:

```powershell
dotnet test TestProject/TestProject.csproj --no-restore --verbosity quiet -consoleloggerparameters:ErrorsOnly -p:GeneratePackageOnBuild=false --filter 'FullyQualifiedName~ProjectControllerContractTests|FullyQualifiedName~LoaderProgressTests' --logger 'trx;LogFileName=load-event-contracts.trx' --results-directory tmp/load-event-verification
```

Обновлены контракты и удалённые типы в диаграммах Model и Project, пояснения API и статус в `ProjectController.md`. Все 23 схемы Project имеют прямые и обратные навигационные ссылки. Локальная структурная проверка 25 изменённых схем подтвердила существование целей ссылок, уникальность псевдонимов и взаимную достижимость схем Project. [Отчёт проверки](../tmp/pdfs/load-event-docs/structural-report.json).

Свежий рендер выполнен автономным локальным PlantUML 1.2026.8 без сетевого доступа: запрос к серверу был отклонён, поэтому серверный рендер этой ревизии не использовался. Проверены 49 схем, входящих в два итоговых PDF вместе с явными зависимостями. Все схемы помещаются в область 1800 × 1600; максимальные ширина и высота — 693 и 1572 пикселя. Хеши исходников соответствуют SVG. [Отчёт рендера](../tmp/pdfs/load-event-docs/render-report.json). Размеры этих схем в таблице ниже обновлены; остальные строки относятся к базовой ревизии 2 октября.

Итоговые PDF Model и Project включают соответственно 80 и 76 страниц, 25 и 24 диаграммы и все соседние описания API. Проверены 684 и 498 внутренних переходов, из них 204 и 106 ссылок со схем; внешних ссылок в итоговых файлах нет. Найдены все 424 и 453 строки сигнатур, схемы остаются векторными, текст находится в пределах страниц. [Проверка Model](../output/pdf/Model.verification.json), [проверка Project](../output/pdf/Project.verification.json). Индексы результатов: [Model-PDF](../Model/Documents/Model-PDF.md), [Project-PDF](../Project/Documents/Project-PDF.md). Проверка одиночных заголовков не обнаружила заголовков без последующего содержимого на той же странице; страниц с одной или двумя строками основного текста также нет. [Отчёт](../tmp/pdfs/load-event-docs/orphan-headings.json).

## Размеры активных схем

Итоговый PDF проекта Model собран отдельно по обновлённым правилам документирования: [Model.pdf](../Model/Documents/Model.pdf), [состав и проверка](../Model/Documents/Model-PDF.md). Проверены 59 страниц, все 205 ссылок исходных диаграмм и 513 внутренних переходов итогового PDF. Включены 21 схема Model, четыре используемые схемы Geometry и полные сигнатуры API; все страницы отрендерены и визуально проверены.

| Диаграмма | Ширина × высота, px |
| --- | --- |
| [GmshApi-1](../GmshApi/Documents/GmshApi-1.mdpuml) | 539 × 819 |
| [GmshApi-10](../GmshApi/Documents/GmshApi-10.mdpuml) | 728 × 1001 |
| [GmshApi-2](../GmshApi/Documents/GmshApi-2.mdpuml) | 539 × 891 |
| [GmshApi-3](../GmshApi/Documents/GmshApi-3.mdpuml) | 541 × 1092 |
| [GmshApi-4](../GmshApi/Documents/GmshApi-4.mdpuml) | 834 × 551 |
| [GmshApi-5](../GmshApi/Documents/GmshApi-5.mdpuml) | 1263 × 810 |
| [GmshApi-6](../GmshApi/Documents/GmshApi-6.mdpuml) | 891 × 919 |
| [GmshApi-7](../GmshApi/Documents/GmshApi-7.mdpuml) | 539 × 543 |
| [GmshApi-8](../GmshApi/Documents/GmshApi-8.mdpuml) | 539 × 376 |
| [GmshApi-9](../GmshApi/Documents/GmshApi-9.mdpuml) | 970 × 677 |
| [GmshApi](../GmshApi/Documents/GmshApi.mdpuml) | 767 × 873 |
| [MaterialDB-1](../MaterialDB/Documents/MaterialDB-1.mdpuml) | 539 × 791 |
| [MaterialDB-2](../MaterialDB/Documents/MaterialDB-2.mdpuml) | 539 × 849 |
| [MaterialDB-3](../MaterialDB/Documents/MaterialDB-3.mdpuml) | 653 × 970 |
| [MaterialDB-4](../MaterialDB/Documents/MaterialDB-4.mdpuml) | 539 × 499 |
| [MaterialDB-5](../MaterialDB/Documents/MaterialDB-5.mdpuml) | 539 × 1014 |
| [MaterialDB-6](../MaterialDB/Documents/MaterialDB-6.mdpuml) | 539 × 1014 |
| [MaterialDB-7](../MaterialDB/Documents/MaterialDB-7.mdpuml) | 539 × 340 |
| [MaterialDB](../MaterialDB/Documents/MaterialDB.mdpuml) | 730 × 681 |
| [Model-1](../Model/Documents/Model-1.mdpuml) | 595 × 1504 |
| [Model-10](../Model/Documents/Model-10.mdpuml) | 478 × 618 |
| [Model-11](../Model/Documents/Model-11.mdpuml) | 478 × 695 |
| [Model-12](../Model/Documents/Model-12.mdpuml) | 478 × 719 |
| [Model-13](../Model/Documents/Model-13.mdpuml) | 478 × 241 |
| [Model-14](../Model/Documents/Model-14.mdpuml) | 606 × 691 |
| [Model-15](../Model/Documents/Model-15.mdpuml) | 478 × 1121 |
| [Model-16](../Model/Documents/Model-16.mdpuml) | 478 × 1109 |
| [Model-17](../Model/Documents/Model-17.mdpuml) | 598 × 1408 |
| [Model-18](../Model/Documents/Model-18.mdpuml) | 478 × 1157 |
| [Model-19](../Model/Documents/Model-19.mdpuml) | 478 × 704 |
| [Model-2](../Model/Documents/Model-2.mdpuml) | 478 × 543 |
| [Model-20](../Model/Documents/Model-20.mdpuml) | 478 × 688 |
| [Model-3](../Model/Documents/Model-3.mdpuml) | 478 × 1161 |
| [Model-4](../Model/Documents/Model-4.mdpuml) | 478 × 720 |
| [Model-5](../Model/Documents/Model-5.mdpuml) | 693 × 1572 |
| [Model-6](../Model/Documents/Model-6.mdpuml) | 478 × 1403 |
| [Model-7](../Model/Documents/Model-7.mdpuml) | 643 × 1446 |
| [Model-8](../Model/Documents/Model-8.mdpuml) | 592 × 1178 |
| [Model-9](../Model/Documents/Model-9.mdpuml) | 478 × 366 |
| [Model](../Model/Documents/Model.mdpuml) | 685 × 1127 |
| [Components](../OperationalController/Documents/Components.mdpuml) | 779 × 325 |
| [ModelView](../OperationalController/Documents/ModelView.mdpuml) | 1217 × 577 |
| [OperationalController-1](../OperationalController/Documents/OperationalController-1.mdpuml) | 675 × 804 |
| [OperationalController-10](../OperationalController/Documents/OperationalController-10.mdpuml) | 628 × 759 |
| [OperationalController-11](../OperationalController/Documents/OperationalController-11.mdpuml) | 719 × 922 |
| [OperationalController-12](../OperationalController/Documents/OperationalController-12.mdpuml) | 745 × 852 |
| [OperationalController-13](../OperationalController/Documents/OperationalController-13.mdpuml) | 698 × 884 |
| [OperationalController-14](../OperationalController/Documents/OperationalController-14.mdpuml) | 628 × 910 |
| [OperationalController-15](../OperationalController/Documents/OperationalController-15.mdpuml) | 628 × 857 |
| [OperationalController-16](../OperationalController/Documents/OperationalController-16.mdpuml) | 628 × 887 |
| [OperationalController-17](../OperationalController/Documents/OperationalController-17.mdpuml) | 628 × 669 |
| [OperationalController-18](../OperationalController/Documents/OperationalController-18.mdpuml) | 628 × 220 |
| [OperationalController-19](../OperationalController/Documents/OperationalController-19.mdpuml) | 712 × 1021 |
| [OperationalController-2](../OperationalController/Documents/OperationalController-2.mdpuml) | 892 × 927 |
| [OperationalController-20](../OperationalController/Documents/OperationalController-20.mdpuml) | 1057 × 543 |
| [OperationalController-21](../OperationalController/Documents/OperationalController-21.mdpuml) | 1058 × 837 |
| [OperationalController-22](../OperationalController/Documents/OperationalController-22.mdpuml) | 983 × 719 |
| [OperationalController-23](../OperationalController/Documents/OperationalController-23.mdpuml) | 876 × 1073 |
| [OperationalController-24](../OperationalController/Documents/OperationalController-24.mdpuml) | 683 × 324 |
| [OperationalController-25](../OperationalController/Documents/OperationalController-25.mdpuml) | 627 × 934 |
| [OperationalController-26](../OperationalController/Documents/OperationalController-26.mdpuml) | 627 × 709 |
| [OperationalController-27](../OperationalController/Documents/OperationalController-27.mdpuml) | 770 × 551 |
| [OperationalController-28](../OperationalController/Documents/OperationalController-28.mdpuml) | 1324 × 1050 |
| [OperationalController-29](../OperationalController/Documents/OperationalController-29.mdpuml) | 939 × 763 |
| [OperationalController-3](../OperationalController/Documents/OperationalController-3.mdpuml) | 586 × 633 |
| [OperationalController-30](../OperationalController/Documents/OperationalController-30.mdpuml) | 627 × 535 |
| [OperationalController-4](../OperationalController/Documents/OperationalController-4.mdpuml) | 617 × 706 |
| [OperationalController-5](../OperationalController/Documents/OperationalController-5.mdpuml) | 599 × 1104 |
| [OperationalController-6](../OperationalController/Documents/OperationalController-6.mdpuml) | 599 × 894 |
| [OperationalController-7](../OperationalController/Documents/OperationalController-7.mdpuml) | 599 × 443 |
| [OperationalController-8](../OperationalController/Documents/OperationalController-8.mdpuml) | 619 × 894 |
| [OperationalController-9](../OperationalController/Documents/OperationalController-9.mdpuml) | 619 × 854 |
| [OperationalController](../OperationalController/Documents/OperationalController.mdpuml) | 1300 × 989 |
| [ProjectController](../OperationalController/Documents/ProjectController.mdpuml) | 1448 × 536 |
| [ProjectEvents](../OperationalController/Documents/ProjectEvents.mdpuml) | 997 × 710 |
| [PostProc-1](../PostProc/Documents/PostProc-1.mdpuml) | 558 × 550 |
| [PostProc-2](../PostProc/Documents/PostProc-2.mdpuml) | 641 × 776 |
| [PostProc-3](../PostProc/Documents/PostProc-3.mdpuml) | 539 × 669 |
| [PostProc-4](../PostProc/Documents/PostProc-4.mdpuml) | 539 × 1097 |
| [PostProc-5](../PostProc/Documents/PostProc-5.mdpuml) | 771 × 894 |
| [PostProc](../PostProc/Documents/PostProc.mdpuml) | 740 × 855 |
| [Geometry-1](../PrGeometry/Documents/Geometry-1.mdpuml) | 478 × 806 |
| [Geometry-2](../PrGeometry/Documents/Geometry-2.mdpuml) | 539 × 981 |
| [Geometry-3](../PrGeometry/Documents/Geometry-3.mdpuml) | 478 × 1115 |
| [Geometry-4](../PrGeometry/Documents/Geometry-4.mdpuml) | 478 × 613 |
| [Geometry-5](../PrGeometry/Documents/Geometry-5.mdpuml) | 478 × 929 |
| [Geometry](../PrGeometry/Documents/Geometry.mdpuml) | 702 × 590 |
| [PreProc-1](../PreProc/Documents/PreProc-1.mdpuml) | 539 × 760 |
| [PreProc](../PreProc/Documents/PreProc.mdpuml) | 503 × 545 |
| [Project-1](../Project/Documents/Project-1.mdpuml) | 478 × 1035 |
| [Project-10](../Project/Documents/Project-10.mdpuml) | 478 × 973 |
| [Project-11](../Project/Documents/Project-11.mdpuml) | 478 × 877 |
| [Project-12](../Project/Documents/Project-12.mdpuml) | 478 × 469 |
| [Project-13](../Project/Documents/Project-13.mdpuml) | 478 × 681 |
| [Project-14](../Project/Documents/Project-14.mdpuml) | 533 × 1023 |
| [Project-15](../Project/Documents/Project-15.mdpuml) | 507 × 976 |
| [Project-16](../Project/Documents/Project-16.mdpuml) | 478 × 1008 |
| [Project-17](../Project/Documents/Project-17.mdpuml) | 478 × 926 |
| [Project-18](../Project/Documents/Project-18.mdpuml) | 478 × 1043 |
| [Project-19](../Project/Documents/Project-19.mdpuml) | 478 × 1057 |
| [Project-2](../Project/Documents/Project-2.mdpuml) | 550 × 950 |
| [Project-20](../Project/Documents/Project-20.mdpuml) | 515 × 902 |
| [Project-21](../Project/Documents/Project-21.mdpuml) | 478 × 1148 |
| [Project-22](../Project/Documents/Project-22.mdpuml) | 478 × 622 |
| [Project-3](../Project/Documents/Project-3.mdpuml) | 539 × 648 |
| [Project-4](../Project/Documents/Project-4.mdpuml) | 478 × 862 |
| [Project-5](../Project/Documents/Project-5.mdpuml) | 478 × 1080 |
| [Project-6](../Project/Documents/Project-6.mdpuml) | 478 × 914 |
| [Project-7](../Project/Documents/Project-7.mdpuml) | 478 × 980 |
| [Project-8](../Project/Documents/Project-8.mdpuml) | 478 × 1056 |
| [Project-9](../Project/Documents/Project-9.mdpuml) | 478 × 518 |
| [Project](../Project/Documents/Project.mdpuml) | 488 × 838 |
| [ResultController](../ResultDataBase/Documents/ResultController.mdpuml) | 738 × 473 |
| [ResultDB-1](../ResultDataBase/Documents/ResultDB-1.mdpuml) | 539 × 661 |
| [ResultDB-2](../ResultDataBase/Documents/ResultDB-2.mdpuml) | 492 × 692 |
| [ResultDB-3](../ResultDataBase/Documents/ResultDB-3.mdpuml) | 539 × 365 |
| [ResultDB](../ResultDataBase/Documents/ResultDB.mdpuml) | 503 × 569 |
| [SurfaceMesher-1](../SurfaceMesher/Documents/SurfaceMesher-1.mdpuml) | 539 × 1050 |
| [SurfaceMesher-2](../SurfaceMesher/Documents/SurfaceMesher-2.mdpuml) | 832 × 818 |
| [SurfaceMesher-3](../SurfaceMesher/Documents/SurfaceMesher-3.mdpuml) | 539 × 688 |
| [SurfaceMesher](../SurfaceMesher/Documents/SurfaceMesher.mdpuml) | 638 × 757 |
