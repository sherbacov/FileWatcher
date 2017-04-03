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

### Режим обработки ошибок
| Название      | Код           | Описание                                                                                  |
| ------------- |:-------------:| ----------------------------------------------------------------------------------------- |
| Error         | 1             | Если при комировании произойдет ошибка, будет произведена попытка выполнения с самого начала. |
| Continue      | 2             | При возникновении ошибки, файл будет пропущен и не скопирован.                            |
| WaitAndRetry  | 3             | Произойдет ожидание RetrySec и затем попытка скопировать файл еще раз. При ошибке произойдет переход к Error |

### Режим перезаписи файлов в папке назначения
| Название      | Код           | Описание                   |
| ------------- |:-------------:| -------------------------- |
| Override      | 1             | Файл будет перезаписан.    |
| Ignore        | 2             | Файл будет проигнорирован. |


### Команды
FileWatcher.exe --help

```
Configuration Result:
[Success] Name FileWather
[Success] Description Служба копирования файлов
[Success] ServiceName FileWather
Topshelf v4.0.0.0, .NET Framework v4.0.30319.42000
Topshelf is a library that simplifies the creation of Windows services using .NET.

Command-Line Reference

  service.exe [verb] [-option:value] [-switch]

    run                 Runs the service from the command line (default)
    help, --help        Displays help

    install             Installs the service

      --autostart       The service should start automatically (default)
      --disabled        The service should be set to disabled
      --manual          The service should be started manually
      --delayed         The service should start automatically (delayed)
      -instance         An instance name if registering the service
                        multiple times
      -username         The username to run the service
      -password         The password for the specified username
      --localsystem     Run the service with the local system account
      --localservice    Run the service with the local service account
      --networkservice  Run the service with the network service permission
      --interactive     The service will prompt the user at installation for
                        the service credentials
      start             Start the service after it has been installed
      --sudo            Prompts for UAC if running on Vista/W7/2008

      -servicename      The name that the service should use when
                        installing
```
