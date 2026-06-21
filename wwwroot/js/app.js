(() => {
    "use strict";

    const BASE_ROUTES = {
        fabricantes: "/api/fabricantes",
        categorias: "/api/categoriasveiculo",
        clientes: "/api/clientes",
        veiculos: "/api/veiculos",
        alugueis: "/api/alugueis"
    };

    const SAMPLE_DATA = {
        fabricantes: [
            { id: 1, nome: "Toyota", paisOrigem: "Japão" },
            { id: 2, nome: "Volkswagen", paisOrigem: "Alemanha" }
        ],
        categorias: [
            { id: 1, nome: "SUV", valorDiariaBase: 180 },
            { id: 2, nome: "Sedan", valorDiariaBase: 150 }
        ],
        clientes: [
            { id: 1, nome: "Pedro Henrique", cpf: "12345678901", email: "pedro@email.com", telefone: "31999999999" },
            { id: 2, nome: "Maria Silva", cpf: "98765432100", email: "maria@email.com", telefone: "31988888888" }
        ],
        veiculos: [
            { id: 1, modelo: "Corolla Cross", anoFabricacao: 2023, quilometragemAtual: 15000, placa: "ABC1D23", fabricanteId: 1, categoriaVeiculoId: 1 },
            { id: 2, modelo: "Virtus", anoFabricacao: 2022, quilometragemAtual: 22000, placa: "XYZ9K88", fabricanteId: 2, categoriaVeiculoId: 2 }
        ],
        alugueis: [
            { id: 1, clienteId: 1, veiculoId: 1, dataInicio: "2026-04-20T08:00:00", dataFimPrevista: "2026-04-23T08:00:00", dataDevolucao: "2026-04-23T10:00:00", quilometragemInicial: 15000, quilometragemFinal: 15320, valorDiaria: 180, valorTotal: 540 }
        ]
    };

    const entityConfig = {
        alugueis: {
            title: "Aluguéis", singular: "aluguel", eyebrow: "Operação", description: "Gerencie contratos, devoluções e valores da operação.",
            filters: [
                { key: "term", label: "Cliente ou veículo", type: "search", placeholder: "Busque por nome ou veículo" },
                { key: "status", label: "Status", type: "select", options: [{ value: "", label: "Todos" }, { value: "active", label: "Ativos" }, { value: "overdue", label: "Atrasados" }, { value: "completed", label: "Finalizados" }] },
                { key: "start", label: "Início a partir de", type: "date" },
                { key: "end", label: "Início até", type: "date" }
            ],
            fields: [
                { key: "clienteId", label: "Cliente", type: "select", source: "clientes", optionLabel: "nome", required: true },
                { key: "veiculoId", label: "Veículo", type: "select", source: "veiculos", optionLabel: "modelo", required: true },
                { key: "dataInicio", label: "Data de início", type: "datetime-local", required: true },
                { key: "dataFimPrevista", label: "Fim previsto", type: "datetime-local", required: true },
                { key: "dataDevolucao", label: "Data de devolução", type: "datetime-local" },
                { key: "quilometragemInicial", label: "Quilometragem inicial", type: "number", min: 0, required: true },
                { key: "quilometragemFinal", label: "Quilometragem final", type: "number", min: 0 },
                { key: "valorDiaria", label: "Valor da diária", type: "number", min: 0, step: "0.01", required: true },
                { key: "valorTotal", label: "Valor total", type: "number", min: 0, step: "0.01", required: true }
            ],
            columns: [
                { label: "Cliente", render: x => `<strong>${escapeHtml(x.cliente || lookup("clientes", x.clienteId))}</strong><small>${escapeHtml(formatCpf(lookupValue("clientes", x.clienteId, "cpf")))}</small>` },
                { label: "Veículo", render: x => `<strong>${escapeHtml(x.veiculo || lookup("veiculos", x.veiculoId, "modelo"))}</strong><small>${escapeHtml(lookupValue("veiculos", x.veiculoId, "placa"))}</small>` },
                { label: "Período", render: x => `<strong>${formatDate(x.dataInicio)}</strong><small>até ${formatDate(x.dataFimPrevista)}</small>` },
                { label: "Status", render: x => statusBadge(x) },
                { label: "Total", className: "align-right", render: x => `<strong>${formatMoney(x.valorTotal)}</strong><small>${formatMoney(x.valorDiaria)}/dia</small>` }
            ],
            predicate: (x, f) => {
                const term = normalize(f.term);
                const haystack = normalize(`${x.cliente || lookup("clientes", x.clienteId)} ${x.veiculo || lookup("veiculos", x.veiculoId, "modelo")}`);
                const started = new Date(x.dataInicio);
                return (!term || haystack.includes(term)) && (!f.status || rentalStatus(x).key === f.status) && (!f.start || started >= startOfDay(f.start)) && (!f.end || started <= endOfDay(f.end));
            }
        },
        veiculos: {
            title: "Veículos", singular: "veículo", eyebrow: "Operação", description: "Controle a frota, a quilometragem e a classificação dos veículos.",
            filters: [
                { key: "term", label: "Modelo ou placa", type: "search", placeholder: "Ex.: Corolla ou ABC1D23" },
                { key: "fabricanteId", label: "Fabricante", type: "select", source: "fabricantes", optionLabel: "nome" },
                { key: "categoriaVeiculoId", label: "Categoria", type: "select", source: "categorias", optionLabel: "nome" },
                { key: "ano", label: "Ano de fabricação", type: "number", placeholder: "Ex.: 2023" }
            ],
            fields: [
                { key: "modelo", label: "Modelo", type: "text", maxlength: 100, required: true, full: true },
                { key: "placa", label: "Placa", type: "text", maxlength: 10, required: true },
                { key: "anoFabricacao", label: "Ano de fabricação", type: "number", min: 1900, max: 2100, required: true },
                { key: "quilometragemAtual", label: "Quilometragem atual", type: "number", min: 0, required: true },
                { key: "fabricanteId", label: "Fabricante", type: "select", source: "fabricantes", optionLabel: "nome", required: true },
                { key: "categoriaVeiculoId", label: "Categoria", type: "select", source: "categorias", optionLabel: "nome", required: true }
            ],
            columns: [
                { label: "Veículo", render: x => `<strong>${escapeHtml(x.modelo)}</strong><small>${x.anoFabricacao}</small>` },
                { label: "Placa", render: x => `<span class="plate">${escapeHtml(x.placa)}</span>` },
                { label: "Fabricante", render: x => escapeHtml(x.fabricante || lookup("fabricantes", x.fabricanteId)) },
                { label: "Categoria", render: x => escapeHtml(x.categoria || lookup("categorias", x.categoriaVeiculoId)) },
                { label: "Quilometragem", className: "align-right", render: x => `<strong>${formatNumber(x.quilometragemAtual)} km</strong>` }
            ],
            predicate: (x, f) => (!f.term || normalize(`${x.modelo} ${x.placa}`).includes(normalize(f.term))) && (!f.fabricanteId || Number(f.fabricanteId) === x.fabricanteId) && (!f.categoriaVeiculoId || Number(f.categoriaVeiculoId) === x.categoriaVeiculoId) && (!f.ano || Number(f.ano) === x.anoFabricacao)
        },
        clientes: {
            title: "Clientes", singular: "cliente", eyebrow: "Operação", description: "Mantenha os dados de contato e identificação dos clientes.",
            filters: [
                { key: "name", label: "Nome", type: "search", placeholder: "Busque pelo nome" },
                { key: "email", label: "E-mail", type: "search", placeholder: "Busque pelo e-mail" },
                { key: "cpf", label: "CPF", type: "search", placeholder: "Somente números" },
                { key: "phone", label: "Telefone", type: "search", placeholder: "DDD ou número" }
            ],
            fields: [
                { key: "nome", label: "Nome completo", type: "text", maxlength: 120, required: true, full: true },
                { key: "cpf", label: "CPF", type: "text", minlength: 11, maxlength: 14, required: true },
                { key: "telefone", label: "Telefone", type: "tel", maxlength: 20 },
                { key: "email", label: "E-mail", type: "email", maxlength: 120, required: true, full: true }
            ],
            columns: [
                { label: "Cliente", render: x => `<strong>${escapeHtml(x.nome)}</strong><small>#${String(x.id).padStart(4, "0")}</small>` },
                { label: "CPF", render: x => escapeHtml(formatCpf(x.cpf)) },
                { label: "E-mail", render: x => escapeHtml(x.email) },
                { label: "Telefone", render: x => escapeHtml(formatPhone(x.telefone) || "—") }
            ],
            predicate: (x, f) => (!f.name || normalize(x.nome).includes(normalize(f.name))) && (!f.email || normalize(x.email).includes(normalize(f.email))) && (!f.cpf || digits(x.cpf).includes(digits(f.cpf))) && (!f.phone || digits(x.telefone).includes(digits(f.phone)))
        },
        fabricantes: {
            title: "Fabricantes", singular: "fabricante", eyebrow: "Cadastros", description: "Organize as marcas utilizadas no cadastro da frota.",
            filters: [
                { key: "name", label: "Nome", type: "search", placeholder: "Busque pelo fabricante" },
                { key: "country", label: "País de origem", type: "search", placeholder: "Ex.: Japão" }
            ],
            fields: [
                { key: "nome", label: "Nome do fabricante", type: "text", maxlength: 100, required: true, full: true },
                { key: "paisOrigem", label: "País de origem", type: "text", maxlength: 60, full: true }
            ],
            columns: [
                { label: "Fabricante", render: x => `<strong>${escapeHtml(x.nome)}</strong><small>#${String(x.id).padStart(3, "0")}</small>` },
                { label: "País de origem", render: x => escapeHtml(x.paisOrigem || "Não informado") },
                { label: "Veículos", className: "align-right", render: x => `<strong>${state.data.veiculos.filter(v => v.fabricanteId === x.id).length}</strong>` }
            ],
            predicate: (x, f) => (!f.name || normalize(x.nome).includes(normalize(f.name))) && (!f.country || normalize(x.paisOrigem).includes(normalize(f.country)))
        },
        categorias: {
            title: "Categorias", singular: "categoria", eyebrow: "Cadastros", description: "Defina classes de veículo e os valores-base das diárias.",
            filters: [
                { key: "name", label: "Nome", type: "search", placeholder: "Busque pela categoria" },
                { key: "min", label: "Diária mínima", type: "number", min: 0, placeholder: "R$ 0,00" },
                { key: "max", label: "Diária máxima", type: "number", min: 0, placeholder: "R$ 500,00" }
            ],
            fields: [
                { key: "nome", label: "Nome da categoria", type: "text", maxlength: 60, required: true, full: true },
                { key: "valorDiariaBase", label: "Valor da diária base", type: "number", min: 0, step: "0.01", required: true, full: true }
            ],
            columns: [
                { label: "Categoria", render: x => `<strong>${escapeHtml(x.nome)}</strong><small>#${String(x.id).padStart(3, "0")}</small>` },
                { label: "Diária base", render: x => `<strong>${formatMoney(x.valorDiariaBase)}</strong>` },
                { label: "Veículos", className: "align-right", render: x => `<strong>${state.data.veiculos.filter(v => v.categoriaVeiculoId === x.id).length}</strong>` }
            ],
            predicate: (x, f) => (!f.name || normalize(x.nome).includes(normalize(f.name))) && (!f.min || x.valorDiariaBase >= Number(f.min)) && (!f.max || x.valorDiariaBase <= Number(f.max))
        }
    };

    class ApiError extends Error {
        constructor(message, status = 0) { super(message); this.status = status; }
    }

    class DataGateway {
        constructor() { this.demoMode = false; this.storageKey = "rota-certa-demo-v2"; }

        async request(url, options = {}) {
            if (this.demoMode) return this.demoRequest(url, options);
            const response = await fetch(url, { headers: { "Content-Type": "application/json" }, ...options });
            const text = await response.text();
            const body = text ? tryJson(text) : null;
            if (!response.ok) throw new ApiError(body?.erro || body?.title || `A API respondeu com status ${response.status}.`, response.status);
            return body;
        }

        enableDemo() {
            this.demoMode = true;
            if (!localStorage.getItem(this.storageKey)) this.saveDemo(structuredClone(SAMPLE_DATA));
        }

        readDemo() { return tryJson(localStorage.getItem(this.storageKey)) || structuredClone(SAMPLE_DATA); }
        saveDemo(data) { localStorage.setItem(this.storageKey, JSON.stringify(data)); }

        async demoRequest(url, options = {}) {
            await wait(90);
            const db = this.readDemo();
            const method = (options.method || "GET").toUpperCase();
            const cleanUrl = url.toLowerCase();

            if (cleanUrl.includes("/api/filtros/")) return this.demoFilter(cleanUrl, db);

            const entity = Object.entries(BASE_ROUTES).find(([, route]) => cleanUrl.startsWith(route))?.[0];
            if (!entity) throw new ApiError("Rota de demonstração não encontrada.", 404);
            const idMatch = cleanUrl.match(/\/(\d+)$/);
            const id = idMatch ? Number(idMatch[1]) : null;

            if (method === "GET") {
                const records = id ? db[entity].find(x => x.id === id) : db[entity];
                if (!records) throw new ApiError("Registro não encontrado.", 404);
                return this.enrich(entity, structuredClone(records), db);
            }

            const payload = options.body ? JSON.parse(options.body) : {};
            if (method === "POST") {
                const nextId = Math.max(0, ...db[entity].map(x => x.id)) + 1;
                const record = { ...payload, id: nextId };
                db[entity].push(record); this.saveDemo(db);
                return structuredClone(record);
            }
            const index = db[entity].findIndex(x => x.id === id);
            if (index < 0) throw new ApiError("Registro não encontrado.", 404);
            if (method === "PUT") {
                db[entity][index] = { ...payload, id }; this.saveDemo(db); return null;
            }
            if (method === "DELETE") {
                this.assertDemoDelete(entity, id, db);
                db[entity].splice(index, 1); this.saveDemo(db); return null;
            }
            throw new ApiError("Operação de demonstração não suportada.", 405);
        }

        enrich(entity, records, db) {
            const enrichOne = record => {
                if (entity === "veiculos") return { ...record, fabricante: db.fabricantes.find(x => x.id === record.fabricanteId)?.nome || "", categoria: db.categorias.find(x => x.id === record.categoriaVeiculoId)?.nome || "" };
                if (entity === "alugueis") return { ...record, cliente: db.clientes.find(x => x.id === record.clienteId)?.nome || "", veiculo: db.veiculos.find(x => x.id === record.veiculoId)?.modelo || "" };
                return record;
            };
            return Array.isArray(records) ? records.map(enrichOne) : enrichOne(records);
        }

        assertDemoDelete(entity, id, db) {
            if (entity === "fabricantes" && db.veiculos.some(x => x.fabricanteId === id)) throw new ApiError("Não é possível excluir o fabricante porque existem veículos vinculados a ele.", 409);
            if (entity === "categorias" && db.veiculos.some(x => x.categoriaVeiculoId === id)) throw new ApiError("Não é possível excluir a categoria porque existem veículos vinculados a ela.", 409);
            if (entity === "clientes" && db.alugueis.some(x => x.clienteId === id)) throw new ApiError("Não é possível excluir o cliente porque existem aluguéis vinculados a ele.", 409);
            if (entity === "veiculos" && db.alugueis.some(x => x.veiculoId === id)) throw new ApiError("Não é possível excluir o veículo porque existem aluguéis vinculados a ele.", 409);
        }

        demoFilter(url, db) {
            if (url.endsWith("veiculos-sem-aluguel")) return this.enrich("veiculos", db.veiculos.filter(v => !db.alugueis.some(a => a.veiculoId === v.id)), db).map(v => ({ veiculoId: v.id, modelo: v.modelo, placa: v.placa, fabricante: v.fabricante }));
            if (url.endsWith("fabricantes-com-total-veiculos")) return db.fabricantes.map(f => ({ fabricanteId: f.id, fabricante: f.nome, totalVeiculos: db.veiculos.filter(v => v.fabricanteId === f.id).length }));
            const id = Number(url.match(/\/(\d+)$/)?.[1]);
            if (url.includes("veiculos-por-fabricante")) return this.enrich("veiculos", db.veiculos.filter(v => v.fabricanteId === id), db);
            if (url.includes("veiculos-por-categoria")) return this.enrich("veiculos", db.veiculos.filter(v => v.categoriaVeiculoId === id), db);
            if (url.includes("alugueis-por-cliente")) return this.enrich("alugueis", db.alugueis.filter(a => a.clienteId === id), db);
            return [];
        }
    }

    const gateway = new DataGateway();
    const state = {
        currentPage: "dashboard",
        currentEntity: "alugueis",
        editId: null,
        pendingDelete: null,
        data: { fabricantes: [], categorias: [], clientes: [], veiculos: [], alugueis: [] },
        reports: { manufacturers: [], availableVehicles: [] }
    };

    const els = {};

    document.addEventListener("DOMContentLoaded", async () => {
        cacheElements();
        bindEvents();
        setCurrentDate();
        await loadData();
        navigate(location.hash.slice(1) || "dashboard", false);
    });

    function cacheElements() {
        ["sidebar", "sidebar-backdrop", "menu-toggle", "main-nav", "page-title", "page-eyebrow", "new-record-button", "entity-new-button", "refresh-button", "connection-status", "page-dashboard", "page-entity", "entity-title", "entity-eyebrow", "entity-description", "filter-form", "clear-filters", "records-title", "result-count", "entity-table-head", "entity-table-body", "empty-state", "record-modal", "record-form", "record-fields", "modal-eyebrow", "modal-title", "modal-description", "form-error", "save-button", "confirm-modal", "confirm-message", "confirm-delete", "manufacturer-chart", "available-vehicles", "available-count", "recent-rentals", "toast-region"].forEach(id => els[toCamel(id)] = document.getElementById(id));
    }

    function bindEvents() {
        els.mainNav.addEventListener("click", event => {
            const link = event.target.closest("[data-page]");
            if (!link) return;
            event.preventDefault(); navigate(link.dataset.page);
        });
        document.addEventListener("click", event => {
            const navigateButton = event.target.closest('[data-action="navigate"]');
            if (navigateButton) navigate(navigateButton.dataset.target);
            const editButton = event.target.closest('[data-action="edit"]');
            if (editButton) openRecordModal(Number(editButton.dataset.id));
            const deleteButton = event.target.closest('[data-action="delete"]');
            if (deleteButton) openDeleteConfirm(Number(deleteButton.dataset.id));
            if (event.target.closest("[data-close-modal]")) els.recordModal.close();
            if (event.target.closest("[data-close-confirm]")) els.confirmModal.close();
        });
        els.newRecordButton.addEventListener("click", () => openRecordModal());
        els.entityNewButton.addEventListener("click", () => openRecordModal());
        els.recordForm.addEventListener("submit", saveRecord);
        els.confirmDelete.addEventListener("click", deleteRecord);
        els.filterForm.addEventListener("input", renderEntityTable);
        els.filterForm.addEventListener("change", renderEntityTable);
        els.clearFilters.addEventListener("click", () => { els.filterForm.reset(); renderEntityTable(); });
        els.refreshButton.addEventListener("click", () => loadData(true));
        els.menuToggle.addEventListener("click", toggleSidebar);
        els.sidebarBackdrop.addEventListener("click", closeSidebar);
        window.addEventListener("hashchange", () => navigate(location.hash.slice(1) || "dashboard", false));
    }

    async function loadData(showFeedback = false) {
        setConnection("loading", "Conectando");
        try {
            const entries = await Promise.all(Object.entries(BASE_ROUTES).map(async ([key, route]) => [key, await gateway.request(route)]));
            entries.forEach(([key, value]) => { state.data[key] = value; });
            setConnection("online", "API conectada");
        } catch (error) {
            gateway.enableDemo();
            const entries = await Promise.all(Object.entries(BASE_ROUTES).map(async ([key, route]) => [key, await gateway.request(route)]));
            entries.forEach(([key, value]) => { state.data[key] = value; });
            setConnection("demo", "Modo demonstração");
            if (showFeedback) toast("API indisponível. Dados de demonstração atualizados.");
        }
        await loadReports();
        renderDashboard();
        if (state.currentPage !== "dashboard") renderEntityPage(state.currentPage);
        if (showFeedback && !gateway.demoMode) toast("Dados atualizados com sucesso.");
    }

    async function loadReports() {
        const results = await Promise.allSettled([
            gateway.request("/api/filtros/fabricantes-com-total-veiculos"),
            gateway.request("/api/filtros/veiculos-sem-aluguel")
        ]);
        state.reports.manufacturers = results[0].status === "fulfilled" ? results[0].value : state.data.fabricantes.map(f => ({ fabricanteId: f.id, fabricante: f.nome, totalVeiculos: state.data.veiculos.filter(v => v.fabricanteId === f.id).length }));
        state.reports.availableVehicles = results[1].status === "fulfilled" ? results[1].value : state.data.veiculos.filter(v => !state.data.alugueis.some(a => a.veiculoId === v.id)).map(v => ({ veiculoId: v.id, modelo: v.modelo, placa: v.placa, fabricante: lookup("fabricantes", v.fabricanteId) }));
    }

    function navigate(page, updateHash = true) {
        const validPage = page === "dashboard" || entityConfig[page] ? page : "dashboard";
        state.currentPage = validPage;
        if (updateHash && location.hash !== `#${validPage}`) history.pushState(null, "", `#${validPage}`);
        document.querySelectorAll(".nav-link").forEach(link => link.classList.toggle("active", link.dataset.page === validPage));
        els.pageDashboard.classList.toggle("active", validPage === "dashboard");
        els.pageEntity.classList.toggle("active", validPage !== "dashboard");
        if (validPage === "dashboard") {
            els.pageTitle.textContent = "Dashboard"; els.pageEyebrow.textContent = "Visão geral"; els.newRecordButton.textContent = "+ Novo aluguel";
            state.currentEntity = "alugueis";
        } else {
            state.currentEntity = validPage; renderEntityPage(validPage);
            const config = entityConfig[validPage];
            els.pageTitle.textContent = config.title; els.pageEyebrow.textContent = config.eyebrow; els.newRecordButton.textContent = `+ Novo ${config.singular}`;
        }
        closeSidebar();
        document.getElementById("app-content").focus({ preventScroll: true });
    }

    function renderDashboard() {
        const now = new Date();
        const active = state.data.alugueis.filter(x => rentalStatus(x).key === "active");
        const overdue = state.data.alugueis.filter(x => rentalStatus(x).key === "overdue");
        document.getElementById("metric-active").textContent = active.length;
        document.getElementById("metric-overdue").textContent = overdue.length ? `${overdue.length} contrato(s) atrasado(s)` : "Nenhum contrato em atraso";
        document.getElementById("metric-vehicles").textContent = state.data.veiculos.length;
        document.getElementById("metric-available").textContent = `${state.reports.availableVehicles.length} sem histórico de aluguel`;
        document.getElementById("metric-clients").textContent = state.data.clientes.length;
        document.getElementById("metric-revenue").textContent = formatMoney(state.data.alugueis.reduce((sum, x) => sum + Number(x.valorTotal || 0), 0));

        const max = Math.max(1, ...state.reports.manufacturers.map(x => x.totalVeiculos));
        els.manufacturerChart.innerHTML = state.reports.manufacturers.length ? state.reports.manufacturers.map(x => `<div class="bar-column"><span class="bar-value">${x.totalVeiculos}</span><div class="bar" style="height:${Math.max(8, x.totalVeiculos / max * 155)}px"></div><span class="bar-label" title="${escapeHtml(x.fabricante)}">${escapeHtml(x.fabricante)}</span></div>`).join("") : emptyCompact("Sem fabricantes cadastrados");

        els.availableCount.textContent = state.reports.availableVehicles.length;
        els.availableVehicles.innerHTML = state.reports.availableVehicles.length ? state.reports.availableVehicles.slice(0, 4).map(x => `<div class="compact-item"><span class="vehicle-badge">${escapeHtml(initials(x.modelo))}</span><div><strong>${escapeHtml(x.modelo)}</strong><small>${escapeHtml(x.fabricante)}</small></div><span class="plate">${escapeHtml(x.placa)}</span></div>`).join("") : emptyCompact("Todos os veículos têm histórico de aluguel");

        const rentals = [...state.data.alugueis].sort((a, b) => new Date(b.dataInicio) - new Date(a.dataInicio)).slice(0, 5);
        els.recentRentals.innerHTML = rentals.length ? rentals.map(x => `<tr><td><strong>${escapeHtml(x.cliente || lookup("clientes", x.clienteId))}</strong></td><td><strong>${escapeHtml(x.veiculo || lookup("veiculos", x.veiculoId, "modelo"))}</strong></td><td><strong>${formatDate(x.dataInicio)}</strong><small>até ${formatDate(x.dataFimPrevista)}</small></td><td>${statusBadge(x)}</td><td class="align-right"><strong>${formatMoney(x.valorTotal)}</strong></td></tr>`).join("") : `<tr><td colspan="5" class="align-right">Nenhum aluguel cadastrado.</td></tr>`;
        void now;
    }

    function renderEntityPage(entity) {
        const config = entityConfig[entity];
        els.entityTitle.textContent = config.title;
        els.entityEyebrow.textContent = config.eyebrow;
        els.entityDescription.textContent = config.description;
        els.entityNewButton.textContent = `+ Novo ${config.singular}`;
        els.recordsTitle.textContent = `Todos os ${config.title.toLowerCase()}`;
        renderFilters(config.filters);
        els.entityTableHead.innerHTML = `<tr>${config.columns.map(c => `<th class="${c.className || ""}">${c.label}</th>`).join("")}<th class="align-right">Ações</th></tr>`;
        renderEntityTable();
    }

    function renderFilters(filters) {
        els.filterForm.innerHTML = filters.map(field => `<div class="field"><label for="filter-${field.key}">${field.label}</label>${inputMarkup(field, `filter-${field.key}`, "filter")}</div>`).join("");
    }

    function renderEntityTable() {
        const config = entityConfig[state.currentEntity];
        if (!config) return;
        const formData = Object.fromEntries(new FormData(els.filterForm).entries());
        const records = state.data[state.currentEntity].filter(record => config.predicate(record, formData));
        els.resultCount.textContent = `${records.length} ${records.length === 1 ? "registro" : "registros"}`;
        els.entityTableBody.innerHTML = records.map(record => `<tr>${config.columns.map(column => `<td class="${column.className || ""}">${column.render(record)}</td>`).join("")}<td><div class="row-actions"><button type="button" class="action-button" data-action="edit" data-id="${record.id}" aria-label="Editar ${escapeHtml(config.singular)}">Editar</button><button type="button" class="action-button delete" data-action="delete" data-id="${record.id}" aria-label="Excluir ${escapeHtml(config.singular)}">Excluir</button></div></td></tr>`).join("");
        els.emptyState.hidden = records.length > 0;
        document.getElementById("entity-table").hidden = records.length === 0;
    }

    async function openRecordModal(id = null) {
        const config = entityConfig[state.currentEntity];
        state.editId = id;
        let record = {};
        if (id) {
            try {
                record = await gateway.request(`${BASE_ROUTES[state.currentEntity]}/${id}`);
            } catch (error) {
                toast(error.message || "Não foi possível carregar o registro.", "error");
                return;
            }
        }
        els.modalEyebrow.textContent = config.eyebrow;
        els.modalTitle.textContent = id ? `Editar ${config.singular}` : `Novo ${config.singular}`;
        els.modalDescription.textContent = id ? "Atualize os dados e confirme para salvar." : "Preencha os campos para criar o cadastro.";
        els.saveButton.textContent = id ? "Salvar alterações" : "Salvar cadastro";
        els.formError.hidden = true;
        els.recordFields.innerHTML = config.fields.map(field => `<div class="field ${field.full ? "full" : ""}"><label for="record-${field.key}">${field.label}</label>${inputMarkup(field, `record-${field.key}`, "record", record[field.key])}</div>`).join("");
        els.recordModal.showModal();
        requestAnimationFrame(() => els.recordFields.querySelector("input, select")?.focus());
    }

    async function saveRecord(event) {
        event.preventDefault();
        if (!els.recordForm.reportValidity()) return;
        const config = entityConfig[state.currentEntity];
        const payload = {};
        new FormData(els.recordForm).forEach((value, key) => { payload[key] = coerceValue(config.fields.find(x => x.key === key), value); });
        if (state.currentEntity === "alugueis") {
            if (new Date(payload.dataFimPrevista) < new Date(payload.dataInicio)) return showFormError("A data final prevista não pode ser menor que a data inicial.");
            if (payload.dataDevolucao && new Date(payload.dataDevolucao) < new Date(payload.dataInicio)) return showFormError("A data de devolução não pode ser menor que a data inicial.");
            if (payload.quilometragemFinal !== null && payload.quilometragemFinal < payload.quilometragemInicial) return showFormError("A quilometragem final não pode ser menor que a inicial.");
        }
        const route = BASE_ROUTES[state.currentEntity] + (state.editId ? `/${state.editId}` : "");
        try {
            els.saveButton.disabled = true; els.saveButton.textContent = "Salvando...";
            await gateway.request(route, { method: state.editId ? "PUT" : "POST", body: JSON.stringify(payload) });
            els.recordModal.close();
            toast(`${capitalize(config.singular)} ${state.editId ? "atualizado" : "cadastrado"} com sucesso.`);
            await reloadAfterMutation();
        } catch (error) {
            showFormError(error.message || "Não foi possível salvar o registro.");
        } finally {
            els.saveButton.disabled = false; els.saveButton.textContent = state.editId ? "Salvar alterações" : "Salvar cadastro";
        }
    }

    function openDeleteConfirm(id) {
        const config = entityConfig[state.currentEntity];
        const record = state.data[state.currentEntity].find(x => x.id === id);
        state.pendingDelete = { entity: state.currentEntity, id };
        const label = record.nome || record.modelo || record.cliente || `#${id}`;
        els.confirmMessage.textContent = `Você está prestes a excluir ${label}. Esta ação não poderá ser desfeita.`;
        els.confirmModal.showModal();
    }

    async function deleteRecord() {
        if (!state.pendingDelete) return;
        const { entity, id } = state.pendingDelete;
        try {
            els.confirmDelete.disabled = true; els.confirmDelete.textContent = "Excluindo...";
            await gateway.request(`${BASE_ROUTES[entity]}/${id}`, { method: "DELETE" });
            els.confirmModal.close(); toast(`${capitalize(entityConfig[entity].singular)} excluído com sucesso.`);
            state.pendingDelete = null; await reloadAfterMutation();
        } catch (error) {
            els.confirmModal.close(); toast(error.message || "Não foi possível excluir o registro.", "error");
        } finally {
            els.confirmDelete.disabled = false; els.confirmDelete.textContent = "Excluir definitivamente";
        }
    }

    async function reloadAfterMutation() {
        const entries = await Promise.all(Object.entries(BASE_ROUTES).map(async ([key, route]) => [key, await gateway.request(route)]));
        entries.forEach(([key, value]) => { state.data[key] = value; });
        await loadReports(); renderDashboard(); renderEntityPage(state.currentEntity);
    }

    function inputMarkup(field, id, scope, value = "") {
        const name = field.key;
        if (field.type === "select") {
            const options = field.source ? [{ value: "", label: field.required ? "Selecione" : "Todos" }, ...state.data[field.source].map(item => ({ value: item.id, label: item[field.optionLabel] }))] : field.options;
            return `<select id="${id}" name="${name}" ${field.required ? "required" : ""}>${options.map(option => `<option value="${escapeHtml(String(option.value))}" ${String(value ?? "") === String(option.value) ? "selected" : ""}>${escapeHtml(option.label)}</option>`).join("")}</select>`;
        }
        const displayValue = field.type === "datetime-local" ? toLocalInput(value) : value ?? "";
        return `<input id="${id}" name="${name}" type="${field.type}" value="${escapeHtml(String(displayValue))}" placeholder="${escapeHtml(field.placeholder || "")}" ${field.required ? "required" : ""} ${attr("min", field.min)} ${attr("max", field.max)} ${attr("step", field.step)} ${attr("maxlength", field.maxlength)} ${attr("minlength", field.minlength)} autocomplete="off" />`;
    }

    function coerceValue(field, value) {
        if (["number"].includes(field.type)) return value === "" ? null : Number(value);
        if (field.type === "select" && field.source) return value === "" ? 0 : Number(value);
        if (field.type === "datetime-local") return value ? new Date(value).toISOString() : null;
        return value || null;
    }

    function lookup(entity, id, field = "nome") { return lookupValue(entity, id, field) || "Não informado"; }
    function lookupValue(entity, id, field) { return state.data[entity].find(x => x.id === id)?.[field] || ""; }
    function rentalStatus(rental) {
        if (rental.dataDevolucao) return { key: "completed", label: "Finalizado" };
        if (new Date(rental.dataFimPrevista) < new Date()) return { key: "overdue", label: "Atrasado" };
        return { key: "active", label: "Ativo" };
    }
    function statusBadge(rental) { const status = rentalStatus(rental); return `<span class="status ${status.key}">${status.label}</span>`; }
    function normalize(value) { return String(value || "").normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase().trim(); }
    function digits(value) { return String(value || "").replace(/\D/g, ""); }
    function formatMoney(value) { return new Intl.NumberFormat("pt-BR", { style: "currency", currency: "BRL", maximumFractionDigits: 2 }).format(Number(value || 0)); }
    function formatNumber(value) { return new Intl.NumberFormat("pt-BR").format(Number(value || 0)); }
    function formatDate(value) { return value ? new Intl.DateTimeFormat("pt-BR").format(new Date(value)) : "—"; }
    function formatCpf(value) { const d = digits(value); return d.length === 11 ? d.replace(/(\d{3})(\d{3})(\d{3})(\d{2})/, "$1.$2.$3-$4") : value || ""; }
    function formatPhone(value) { const d = digits(value); return d.length === 11 ? d.replace(/(\d{2})(\d{5})(\d{4})/, "($1) $2-$3") : value || ""; }
    function startOfDay(value) { return new Date(`${value}T00:00:00`); }
    function endOfDay(value) { return new Date(`${value}T23:59:59`); }
    function toLocalInput(value) { if (!value) return ""; const d = new Date(value); d.setMinutes(d.getMinutes() - d.getTimezoneOffset()); return d.toISOString().slice(0, 16); }
    function initials(value) { return String(value || "V").split(" ").slice(0, 2).map(x => x[0]).join("").toUpperCase(); }
    function capitalize(value) { return value.charAt(0).toUpperCase() + value.slice(1); }
    function escapeHtml(value) { return String(value ?? "").replace(/[&<>'"]/g, char => ({ "&": "&amp;", "<": "&lt;", ">": "&gt;", "'": "&#39;", '"': "&quot;" }[char])); }
    function attr(name, value) { return value === undefined ? "" : `${name}="${escapeHtml(String(value))}"`; }
    function toCamel(value) { return value.replace(/-([a-z])/g, (_, letter) => letter.toUpperCase()); }
    function tryJson(value) { try { return JSON.parse(value); } catch { return null; } }
    function wait(ms) { return new Promise(resolve => setTimeout(resolve, ms)); }
    function emptyCompact(message) { return `<div class="empty-state"><span>◇</span><p>${escapeHtml(message)}</p></div>`; }

    function setConnection(mode, text) { els.connectionStatus.className = `connection-status ${mode}`; els.connectionStatus.innerHTML = `<i></i> ${escapeHtml(text)}`; }
    function setCurrentDate() { const node = document.querySelector(".welcome-row .eyebrow"); node.textContent = capitalize(new Intl.DateTimeFormat("pt-BR", { weekday: "long", day: "2-digit", month: "long" }).format(new Date())); }
    function showFormError(message) { els.formError.textContent = message; els.formError.hidden = false; }
    function toast(message, type = "success") { const node = document.createElement("div"); node.className = `toast ${type}`; node.innerHTML = `<span>${type === "error" ? "!" : "✓"}</span><span>${escapeHtml(message)}</span>`; els.toastRegion.appendChild(node); setTimeout(() => node.remove(), 4200); }
    function toggleSidebar() { const open = !els.sidebar.classList.contains("open"); els.sidebar.classList.toggle("open", open); els.sidebarBackdrop.classList.toggle("open", open); els.menuToggle.setAttribute("aria-expanded", String(open)); }
    function closeSidebar() { els.sidebar.classList.remove("open"); els.sidebarBackdrop.classList.remove("open"); els.menuToggle.setAttribute("aria-expanded", "false"); }
})();
