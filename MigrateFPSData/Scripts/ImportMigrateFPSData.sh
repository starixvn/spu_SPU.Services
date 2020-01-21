#!/bin/bash
$(MYSQL_PWD=fps@1qaz@Wsx# mysql -u "fps" -D "FPS_RawTransaction" -e "LOAD DATA LOCAL INFILE '/var/www/FPSData/$1' 
                                                              INTO TABLE RawTransaction 
                                                              FIELDS TERMINATED BY '|' 
                                                              LINES TERMINATED BY '\r\n' 
                                                              (LoginName,SubscriberName,TransTime,CreatedDate,DeviceCode,FingerprintCode,FingerprintMoreInfo,UserAgent,IP,IPID,PluginID,URL,Action,ActionResult,InvalidDevice,TransStatus);")
