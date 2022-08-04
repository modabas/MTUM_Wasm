-- Table: public.tbl_tenant

-- DROP TABLE IF EXISTS public.tbl_tenant;

CREATE TABLE IF NOT EXISTS public.tbl_tenant
(
    id uuid NOT NULL DEFAULT gen_random_uuid(),
    name text COLLATE pg_catalog."default" NOT NULL,
    name_lower text COLLATE pg_catalog."default" NOT NULL GENERATED ALWAYS AS (lower(name)) STORED,
    is_enabled boolean NOT NULL DEFAULT true,
    created_at timestamp with time zone NOT NULL DEFAULT now(),
    last_modified_at timestamp with time zone NOT NULL DEFAULT now(),
    nac_policy jsonb,
    CONSTRAINT tbl_tenant_pkey PRIMARY KEY (id)
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.tbl_tenant
    OWNER to postgres;
-- Index: ix_tbl_tenant_name_lower

-- DROP INDEX IF EXISTS public.ix_tbl_tenant_name_lower;

CREATE UNIQUE INDEX IF NOT EXISTS ix_tbl_tenant_name_lower
    ON public.tbl_tenant USING btree
    (name_lower COLLATE pg_catalog."default" ASC NULLS LAST)
    TABLESPACE pg_default;



-- Table: public.tbl_audit_log

-- DROP TABLE IF EXISTS public.tbl_audit_log;

CREATE TABLE IF NOT EXISTS public.tbl_audit_log
(
    id bigint NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 9223372036854775807 CACHE 1 ),
    command_name text COLLATE pg_catalog."default" NOT NULL,
    email text COLLATE pg_catalog."default" NOT NULL GENERATED ALWAYS AS ((logged_in_user ->> 'EmailAddress'::text)) STORED,
    tenant_id uuid GENERATED ALWAYS AS (((logged_in_user ->> 'TenantId'::text))::uuid) STORED,
    remote_ip text COLLATE pg_catalog."default" NOT NULL,
    created_at timestamp with time zone NOT NULL DEFAULT now(),
    created_date date NOT NULL DEFAULT (now())::date,
    logged_in_user jsonb NOT NULL,
    entry jsonb NOT NULL,
    CONSTRAINT tbl_audit_log_pkey PRIMARY KEY (created_date, id),
    CONSTRAINT tbl_audit_log_tenant_id FOREIGN KEY (tenant_id)
        REFERENCES public.tbl_tenant (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE RESTRICT
) PARTITION BY RANGE (created_date);

ALTER TABLE IF EXISTS public.tbl_audit_log
    OWNER to postgres;
-- Index: fki_tbl_audit_log_tenant_id

-- DROP INDEX IF EXISTS public.fki_tbl_audit_log_tenant_id;

CREATE INDEX IF NOT EXISTS fki_tbl_audit_log_tenant_id
    ON public.tbl_audit_log USING btree
    (tenant_id ASC NULLS LAST)
;
-- Index: ix_tbl_audit_log_created_at_and_created_date

-- DROP INDEX IF EXISTS public.ix_tbl_audit_log_created_at_and_created_date;

CREATE INDEX IF NOT EXISTS ix_tbl_audit_log_created_at_and_created_date
    ON public.tbl_audit_log USING btree
    (created_at DESC NULLS LAST, created_date ASC NULLS LAST)
;
-- Index: ix_tbl_audit_log_created_at_and_created_date_and_tenant_id

-- DROP INDEX IF EXISTS public.ix_tbl_audit_log_created_at_and_created_date_and_tenant_id;

CREATE INDEX IF NOT EXISTS ix_tbl_audit_log_created_at_and_created_date_and_tenant_id
    ON public.tbl_audit_log USING btree
    (created_at DESC NULLS LAST, created_date ASC NULLS LAST, tenant_id ASC NULLS LAST)
;





