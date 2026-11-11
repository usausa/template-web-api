# Template project for Web API

[English](README.md) | 日本語

[Template](https://usausa.github.io/template/) project for Web API.

# ✅ 実装

| 分類 | 機能 |
| --- | --- |
| API | Minimal API / 版(`/api/v1`) / OpenAPI(Swagger UI・ReDoc) / 検証 / ProblemDetails |
| 認証 | JWT / API キー / ロールポリシー |
| データ | SQLite(Smart.Data.Accessor) / ページング・並べ替え・検索 / 楽観的同時実行制御(ETag) / ファイル保管 |
| HTTP | レート制限 / 圧縮(Brotli・Gzip) / セキュリティヘッダー / 転送ヘッダー |
| 監視 | Serilog / HTTP ログ / W3C ログ / SQL プロファイラー(MiniDataProfiler) / OpenTelemetry(OTLP・Prometheus) / ヘルスチェック |
| 運用 | 機能フラグ / Windows サービス / systemd / Aspire AppHost |

# 🔗 関連プロジェクト

| プロジェクト | 説明 |
| --- | --- |
| [template-web-fullstack](https://github.com/usausa/template-web-fullstack) | フルスタック構成のテンプレート |
| [template-web-mvc](https://github.com/usausa/template-web-mvc) | ASP.NET Core MVC のテンプレート(Identity・パスキー) |
| [template-blazor-server](https://github.com/usausa/template-blazor-server) | Blazor Server アプリのテンプレート |
| [template-blazor-wasm](https://github.com/usausa/template-blazor-wasm) | Blazor WebAssembly + ASP.NET Core API のテンプレート |
| [template-grpc-server](https://github.com/usausa/template-grpc-server) | gRPC サーバーのテンプレート |

# 📦 自作ライブラリ

| ライブラリ | 説明 |
| --- | --- |
| [Usa.Smart.Data.Accessor](https://github.com/usausa/Smart-Net-Data-Accessor) | データアクセス |
| [Usa.Smart.Mapper](https://github.com/usausa/Smart-Net-Mapper) | オブジェクトマッパー |
| [MiniDataProfiler](https://github.com/usausa/mini-data-profiler) | SQL プロファイラー |
| [BunnyTail.CommonCode](https://github.com/usausa/bunnytail-common-code) | 共通コード |
| [BunnyTail.ServiceRegistration](https://github.com/usausa/bunnytail-service-registration) | サービス登録 |
