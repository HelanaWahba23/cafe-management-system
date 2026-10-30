# Public interfaces

GET /: anonymous Café homepage. GET /#about: same page about section. GET /menu/pickup and /menu/t/{code}: existing functional menus. Existing private tracking remains capability-scoped. /account/login is labelled staff access, never customer signup. No new API or database contract.

GET /images/cafe-hero.png: local static decorative asset. CSS/header/footer changes are scoped to PublicLayout. All existing endpoints and authorization/privacy rules remain except root's replacement of its prior redirect with the homepage.
