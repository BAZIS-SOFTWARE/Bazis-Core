# Первый этап реализации задачи #327

Дата: 14 октября 2026 года.
Задача: https://github.com/ONEGOCH/CoreSolution/issues/327
Ветвь: `codex/327-fix-curve-mesh-change-scope` (создана локально и на GitHub от 7c9e5706).

В четырёх публичных методах экструзии по сетке кривых добавлена область `BeginChanges()`. Для Adjusted она охватывает создание объёма и оба слияния узлов. Проверка допуска осталась перед областью. Исправлен XML-комментарий RegisterChange. Алгоритмы экструзии не изменены.

Добавлено семь интеграционных сценариев CurveMeshExtrusionTests с настоящей gmsh.dll: две перегрузки 3D и Adjusted в обоих режимах transfinite, а также 1D. Проверены создание элементов и узлов, один итоговый пакет, уведомления о созданном наборе и узлах, освобождение области после ошибки. Обработчик Adjusted проверяет отсутствие совпадающих узлов и удаление близкого узла существующей сетки после присоединения.

## Проверка

Сборка TestProject и его зависимостей успешна. CurveMeshExtrusionTests, ProjectControllerContractTests, GeometryBatchChangeTests и BulkDataChangeTests: **52/52 успешно, без пропусков**. Устаревший TestModel не собирался. Проверка `git diff --check` прошла.

```powershell
dotnet test TestProject/TestProject.csproj --no-restore -p:GeneratePackageOnBuild=false --filter 'FullyQualifiedName~CurveMeshExtrusionTests|FullyQualifiedName~ProjectControllerContractTests|FullyQualifiedName~GeometryBatchChangeTests|FullyQualifiedName~BulkDataChangeTests' --verbosity quiet -consoleloggerparameters:ErrorsOnly --logger 'trx;LogFileName=curve-mesh.trx' --results-directory tmp/curve-mesh-verification
```

[Результат тестов](../tmp/curve-mesh-verification/curve-mesh.trx).

## Оставшиеся этапы

- В OperationalController-20 обновлены сигнатуры и описание области изменений; свежий рендер и итоговый PDF пока не подготовлены.
- Версия пакета не изменена, выпуск не выполнен. Simmax по-прежнему использует прежнюю пакетную зависимость.
- Полный сценарий мастера Simmax с несколькими проходами и швами пока не проверен.
- Изменения реализации остаются в рабочем дереве; созданная ветвь GitHub пока указывает на исходный коммит.

Существующая пользовательская правка `.githooks/pull_large_files.cmd` сохранена.
