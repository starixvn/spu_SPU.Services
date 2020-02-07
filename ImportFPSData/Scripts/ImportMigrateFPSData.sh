#!/bin/bash
$(MYSQL_PWD=fps@1qaz@Wsx# mysql -u "fps" -D "DCS_RawTransaction" -e "LOAD DATA LOCAL INFILE '/home/vnteam/FPSData/$1/$2' 
                                                              INTO TABLE DCS_RawTransaction.RawTransaction                                                              
                                                              FIELDS TERMINATED BY '|'                                                               
                                                              LINES TERMINATED BY '\n'                              
                                                              (LoginName,SubscriberName,TransTime,CreatedDate,DeviceCode,FingerprintCode,FingerprintMoreInfo,UserAgent,IP,IPID,PluginID,@url,Action,ActionResult,InvalidDevice,@transStatus)
								SET TransStatus=cast(@transStatus as UNSIGNED INTEGER),
								URL = SUBSTRING_INDEX(@url, '?', 1)");
