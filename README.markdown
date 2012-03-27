Custom trace listener for writing messages from the Enterprise Library Logging Application Block to [Loggly.com](http://loggly.com).

### Usage:

```c#
<configuration>
    <loggingConfiguration name="" tracingEnabled="true" defaultCategory="General">
        <listeners>
		    ...
            <add name="Loggly Listener" type="Loggly.EntLib.TraceListeners.LogglyTraceListenerData, Loggly.EntLib.TraceListeners, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"
                listenerDataType="Loggly.EntLib.TraceListeners.LogglyTraceListenerData, Loggly.EntLib.TraceListeners"
                source="Enterprise Library Logging" formatter="Text Formatter"
                log="" machineName="." traceOutputOptions="None"
                logglyInputKey="my-long-key-that-i-got-when-setting-up-my-http-input" />
        </listeners>
	</loggingConfiguration>
	...
</configuration>
```