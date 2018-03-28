# Carrega o snap-in do SharePoint.
Add-PsSnapin Microsoft.SharePoint.PowerShell;

# Verifica se o ambiente realmente está utilizando o ViewState cacheada pelo serviço
# de cache distribuído do SharePoint.
$ContentService = [Microsoft.SharePoint.Administration.SPWebService]::ContentService;
$ContentService.ViewStateOnServer; # true = ViewState está sendo jogado no cache.

# Recupera as configurações do serviço de Cache Distribuído (aka AppFabric) do SharePoint
# relativas ao caching de ViewState.
$viewStateCacheSettings = Get-SPDistributedCacheClientSetting -ContainerType DistributedViewStateCache;

# Imprime na tela as configurações atuais do serviço de Cache Distribuído de ViewState.
$viewStateCacheSettings;

# Redefine os valores para as configurações de Cache Distribuído de ViewState de acordo com os
# parâmetros sugeridos pela própria documentação do AppFabric, muito menos agressivas do que os
# valores padrão dos ambientes SharePoint.
# https://msdn.microsoft.com/en-us/library/hh351483(v=azure.10).aspx
$viewStateCacheSettings.ChannelOpenTimeOut = 3000; #20
$viewStateCacheSettings.ReceiveTimeout = 60000; #60000
$viewStateCacheSettings.RequestTimeout = 1000;  #20
Set-SPDistributedCacheClientSetting -ContainerType DistributedViewStateCache $viewStateCacheSettings;

# Executa o mesmo procedimento para o Cache Distribuído de Logon Token, que também pode influenciar
# na perda de ViewStates do Cache.
# https://blogs.msdn.microsoft.com/sambetts/2015/02/17/appfabric-distributed-logon-tokenviewstate-cache-is-timing-out-continued/
$logonTokenCacheSettings = Get-SPDistributedCacheClientSetting -ContainerType DistributedLogonTokenCache;

# Imprime na tela as configurações atuais do serviço de Cache Distribuído de Logon Token.
$logonTokenCacheSettings;

# Redefine os valores para as configurações de acordo com os valores sugeridos pela própria documentação
# do AppFabric.
$logonTokenCacheSettings.ChannelOpenTimeOut = 3000;
$logonTokenCacheSettings.ReceiveTimeout = 60000;
$logonTokenCacheSettings.RequestTimeout = 1000;
Set-SPDistributedCacheClientSetting -ContainerType DistributedLogonTokenCache $logonTokenCacheSettings;

# Reconfigura o TTL (Time To Live) do cache da ViewState para que a mesma não seja perdida após 10 minutos.
# Seleciona o cluster de cache atual.
Use-CacheCluster;

# Obtém o cache da ViewState do AppFabric (Distributed Cache);
$distributedViewStateCache = Get-AFCache | ? { $_.CacheName.StartsWith("DistributedViewStateCache") };

# Imprime na tela as configurações do cache de ViewState.
# O valor padrão para tempo de vida (TTL) deste cache é de 10 minutos.
$viewStateCacheConfig = Get-AFCacheConfiguration -CacheName $distributedViewStateCache.CacheName;
$viewStateCacheConfig;

# Altera o tempo de vida do cache de ViewState para 60 minutos (uma hora).
Stop-CacheCluster;
Set-AFCacheConfiguration -CacheName $distributedViewStateCache.CacheName -TimeToLiveMins 60;
Start-CacheCluster;

# Executa o mesmo procedimento para o tempo de vida do cache de Logon Token.
$distributedLogonTokenCache = Get-AFCache | ? { $_.CacheName.StartsWith("DistributedLogonTokenCache") };
$logonTokenCacheConfig = Get-AFCacheConfiguration -CacheName $distributedLogonTokenCache.CacheName;
$viewStateCacheConfig;
Stop-CacheCluster;
Set-AFCacheConfiguration -CacheName $distributedLogonTokenCache.CacheName -TimeToLiveMins 60;
Start-CacheCluster; 
