Диаграмма прецедентов

@startuml
left to right direction

rectangle Simmax-server {
  usecase "Загрузка лицензий" as UC1
}
rectangle Simmax-client {
  usecase "Загрузка CAD моделей" as UC2
  usecase "Отправка задач на расчет" as UC3
  usecase "Вывод результатов" as UC4
  usecase UC5 as "Постановка задачи выращивания:
  --
  построение сетки, \n задание граничных условий, \n выбор стратегии спекания, \n задание толщины слоя"
  usecase UC6 as "Постановка задачи термообработки:
  --
  задание граничных условий"

}

@enduml
---
* для справок