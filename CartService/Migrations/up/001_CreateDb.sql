-- Identity DB ---------------------------------------
CREATE DATABASE "identity.db"
    WITH 
    OWNER = postgres
    ENCODING = 'UTF8'
    LC_COLLATE = 'en_US.utf8'
    LC_CTYPE = 'en_US.utf8'
    TABLESPACE = pg_default
    CONNECTION LIMIT = -1;

-- Sheduler DB ---------------------------------------
CREATE DATABASE "store.sheduler.db"
    WITH 
    OWNER = postgres
    ENCODING = 'UTF8'
    LC_COLLATE = 'en_US.utf8'
    LC_CTYPE = 'en_US.utf8'
    TABLESPACE = pg_default
    CONNECTION LIMIT = -1;

-- Carts Service DB ----------------------------------
CREATE DATABASE "store.cart.db"
    WITH 
    OWNER = postgres
    ENCODING = 'UTF8'
    LC_COLLATE = 'en_US.utf8'
    LC_CTYPE = 'en_US.utf8'
    TABLESPACE = pg_default
    CONNECTION LIMIT = -1;

-- products 
CREATE TABLE public.products
(
    id integer NOT NULL,
    name text COLLATE pg_catalog."default" NOT NULL,
    cost numeric NOT NULL,
    for_bonus_points boolean NOT NULL DEFAULT false,
    CONSTRAINT products_pkey PRIMARY KEY (id)
)

TABLESPACE pg_default;

ALTER TABLE public.products
    OWNER to postgres;

-- carts 
CREATE TABLE public.carts
(
    created date NOT NULL DEFAULT now(),
    "number" integer NOT NULL DEFAULT 1,
    product_id integer NOT NULL,
    buyer_id uuid NOT NULL,
    edited date NOT NULL DEFAULT now(),
    CONSTRAINT uk_catrs_product_id_buyer_id UNIQUE (product_id, buyer_id),
    CONSTRAINT fk_product_id_products_id FOREIGN KEY (product_id)
        REFERENCES public.products (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
)

TABLESPACE pg_default;

ALTER TABLE public.carts
    OWNER to postgres;


-- webhooks
CREATE TABLE public.webhooks
(
    buyer_id uuid,
    url text COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT uk_webhooks_buyer_id_url UNIQUE (buyer_id, url)
)

TABLESPACE pg_default;

ALTER TABLE public.webhooks
    OWNER to postgres;

------------------------------------------------------