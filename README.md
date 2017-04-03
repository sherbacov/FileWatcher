# FileWatcher
Программа копирования файлов из нескольких источников в несколько назначений.

```xml
  <castle>
    <components>
      <component id="Config">
        <parameters>
          <SourceFolder>c:\src</SourceFolder><!-- папка источник. поддержка нескольких папков c:\src;c:\src2 -->
          <DestFolder>c:\dst</DestFolder><!-- папка назначение. поддержка нескольких папков c:\dst;c:\dst2 -->
          <Mode>1</Mode><!-- режим обработки ошибок. см. ниже -->
          <FileExist>1</FileExist><!-- режим обработки файлов назначения. см. ниже -->
          <RetrySec>30</RetrySec>
          <WaitSecs>5</WaitSecs><!-- время ожидания разблокировки исходного файла -->
          <StartBackupThread>True</StartBackupThread>
        </parameters>
      </component>
    </components>
  </castle>
```

# Режим обработки ошибок
| Название      | Код           | Описание                                                                                  |
| ------------- |:-------------:| -----------------------------------------------------------------------------------------:|
| Error         | 1             | При комировании произойдет ошибка и будет произведена попытка выполнения с самого начала. |
| Continue      | 2             | При возникновении ошибки, файл будет пропущен и не скопирован.                            |
| WaitAndRetry  | 3             | Произойдет ожидание RetrySec и затем попытка скопировать файл еще раз. При ошибке произойдет переход к Error |
