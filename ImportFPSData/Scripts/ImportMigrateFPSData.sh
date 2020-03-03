#!/bin/bash
$(MYSQL_PWD=fps@1qaz@Wsx# mysql -u "fps" -D "DCS_RawTransaction" -e "LOAD DATA LOCAL INFILE '/home/vnteam/FPSData/$1/$2' 
                                                              INTO TABLE DCS_RawTransaction.RawTransaction                                                              
                                                              FIELDS TERMINATED BY '|'                                                               
                                                              LINES TERMINATED BY '\n'                              
                                                              (LoginName,SubscriberName,@transTime,CreatedDate,@deviceCode,@fingerprintCode,FingerprintMoreInfo,@userAgent,@IP,IPID,PluginID,@url,@action,@actionResult,@invalidDevice,@transStatus,FPSTransID)
								SET 
									TransTime = @transTime,
									CreatedDate = STR_TO_DATE(@transTime,'%Y-%m-%d'),
									TransStatus=cast(@transStatus as UNSIGNED INTEGER),
		      						URL = @url,
									DeviceCode = case when @deviceCode = '' then NULL else @deviceCode end,
									FingerprintCode = case when @fingerprintCode = '' then NULL else @fingerprintCode end,
									UserAgent = case when @userAgent = '' then NULL else @userAgent end,
									IP = case when @IP = '' then NULL else @IP end,
									Action = case when @action = '' then NULL else @action end,
									ActionResult = case when @actionResult = '' then NULL else @actionResult end,
									InvalidDevice = case when @invalidDevice = '' then NULL else @invalidDevice end");
