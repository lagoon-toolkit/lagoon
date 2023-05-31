self.applicationRootName = '{APP_ROOT_NAME}';
self.importScripts('./service-worker-assets.js?v={GUID}');{SERVICE_WORKER_SETUP}
self.importScripts("./_content/Lagoon.UI/js/service-worker-shared.js?v={GUID}");
self.importScripts('./_content/Lagoon.UI/js/service-worker-handler.js?v={GUID}');
