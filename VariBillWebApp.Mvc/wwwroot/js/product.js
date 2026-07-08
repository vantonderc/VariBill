const API_BASE = '/catalog';
let __antiforgeryToken = null;
let __antiforgeryHeaderName = 'RequestVerificationToken';

async function fetchAntiforgeryToken() {
    try {
        const res = await fetch(`${API_BASE}/antiforgery/token`, { method: 'GET', headers: { 'Accept': 'application/json' } });
        if (!res.ok) return;
        const body = await res.json();
        __antiforgeryToken = body?.token ?? null;
        __antiforgeryHeaderName = body?.headerName ?? __antiforgeryHeaderName;
    } catch (e) {
        // ignore silently; server-side calls (non-AJAX) still work
    }
}

async function api(path, method = 'GET', body = null)
{
    const opts = {
        method,
        headers: { 'Content-Type': 'application/json', 'Accept': 'application/json' }
    };
    if (body) opts.body = JSON.stringify(body);
    // include antiforgery token for state-changing requests when available
    if (__antiforgeryToken && ['POST','PUT','DELETE'].includes(method.toUpperCase())) {
        opts.headers[__antiforgeryHeaderName] = __antiforgeryToken;
    }

    const res = await fetch(`${API_BASE}${path}`, opts);

    // Handle authentication errors
    if (res.status === 401)
    {
        // Not logged in – redirect to login
        window.location.href = '/Account/Login';
        throw new Error('Please log in.');
    }
    if (res.status === 403)
    {
        showMessage('message', 'You do not have permission to perform this action.', 'error');
        throw new Error('Forbidden');
    }

    if (!res.ok)
    {
        let message = `HTTP ${res.status}`;
        try
        {
            const errorBody = await res.json();
            if (errorBody?.message) message = errorBody.message;
        } catch { /* ignore non-json */ }
        throw new Error(message);
    }

    return res.status === 204 ? null : res.json();
}

function escHtml(str)
{
    return String(str ?? '').replace(/[&<>"']/g, c =>
        ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[c]));
}

function showMessage(elementId, text, type)
{
    const element = document.getElementById(elementId);
    element.textContent = text;
    element.className = `message ${type}`;
    setTimeout(() => { element.className = 'message'; }, 4000);
}

async function loadProductTypes()
{
    document.getElementById('product-types-loading').style.display = 'block';
    document.getElementById('product-types-table').style.display = 'none';

    const types = await api('/producttypes');
    renderProductTypes(types);
    populateProductTypeSelect(types);
}

function renderProductTypes(types)
{
    const tbody = document.getElementById('product-types-body');
    tbody.innerHTML = types.length === 0
        ? '<tr><td colspan="4" style="text-align:center;color:#999">No product types found.</td></tr>'
        : types.map(t => `
            <tr>
                <td><strong>${escHtml(t.name)}</strong></td>
                <td>${escHtml(t.description ?? '')}</td>
                <td>${t.productCount ?? 0}</td>
                <td>
                    <button class="btn btn-edit" onclick="editProductType('${t.id}')">Edit</button>
                    <button class="btn btn-danger" onclick="deleteProductType('${t.id}')">Delete</button>
                </td>
            </tr>`).join('');

    document.getElementById('product-types-loading').style.display = 'none';
    document.getElementById('product-types-table').style.display = 'table';
}

function populateProductTypeSelect(types)
{
    const sel = document.getElementById('product-type');
    const current = sel.value;
    sel.innerHTML = '<option value="">Select type...</option>' +
        types.map(t => `<option value="${t.id}">${escHtml(t.name)}</option>`).join('');
    if (current) sel.value = current;
}

async function loadProducts()
{
    document.getElementById('loading').style.display = 'block';
    document.getElementById('products-table').style.display = 'none';

    const products = await api('/products');
    renderProducts(products);
}

function renderProducts(products)
{
    const tbody = document.getElementById('products-body');
    tbody.innerHTML = products.length === 0
        ? '<tr><td colspan="5" style="text-align:center;color:#999">No products found.</td></tr>'
        : products.map(p => `
            <tr>
                <td><strong>${escHtml(p.name)}</strong></td>
                <td>${escHtml(p.productTypeName ?? '')}</td>
                <td>R ${Number(p.price).toFixed(2)}</td>
                <td>${escHtml(p.description ?? '')}</td>
                <td>
                    <button class="btn btn-edit" onclick="editProduct('${p.id}')">Edit</button>
                    <button class="btn btn-danger" onclick="deleteProduct('${p.id}')">Delete</button>
                </td>
            </tr>`).join('');

    document.getElementById('loading').style.display = 'none';
    document.getElementById('products-table').style.display = 'table';
}

async function saveProduct()
{
    const id = document.getElementById('product-id').value;
    const dto = {
        name: document.getElementById('name').value.trim(),
        price: parseFloat(document.getElementById('price').value),
        productTypeId: document.getElementById('product-type').value,
        description: document.getElementById('description').value.trim() || null
    };

    if (!dto.name || !dto.productTypeId || Number.isNaN(dto.price) || dto.price <= 0)
    {
        showMessage('message', 'Please complete all required product fields.', 'error');
        return;
    }

    try
    {
        if (id) await api(`/products/${id}`, 'PUT', dto);
        else await api('/products', 'POST', dto);

        showMessage('message', 'Product saved successfully.', 'success');
        clearForm();
        await loadProducts();
    } catch (err)
    {
        showMessage('message', 'Error saving product: ' + err.message, 'error');
    }
}

async function editProduct(id)
{
    const p = await api(`/products/${id}`);
    document.getElementById('product-id').value = p.id;
    document.getElementById('name').value = p.name;
    document.getElementById('price').value = p.price;
    document.getElementById('product-type').value = p.productTypeId;
    document.getElementById('description').value = p.description ?? '';
    document.getElementById('form-title').textContent = 'Edit Product';
}

async function deleteProduct(id)
{
    if (!confirm('Are you sure you want to delete this product?')) return;

    try
    {
        await api(`/products/${id}`, 'DELETE');
        await loadProducts();
    } catch (err)
    {
        showMessage('message', 'Error deleting product: ' + err.message, 'error');
    }
}

function clearForm()
{
    document.getElementById('product-id').value = '';
    document.getElementById('name').value = '';
    document.getElementById('price').value = '';
    document.getElementById('description').value = '';
    document.getElementById('product-type').value = '';
    document.getElementById('form-title').textContent = 'Add / Edit Product';
}

async function saveProductType()
{
    const id = document.getElementById('product-type-id').value;
    const dto = {
        name: document.getElementById('product-type-name').value.trim(),
        description: document.getElementById('product-type-description').value.trim() || null
    };

    if (!dto.name)
    {
        showMessage('product-type-message', 'Product type name is required.', 'error');
        return;
    }

    try
    {
        if (id) await api(`/producttypes/${id}`, 'PUT', dto);
        else await api('/producttypes', 'POST', dto);

        showMessage('product-type-message', 'Product type saved successfully.', 'success');
        clearProductTypeForm();
        await loadProductTypes();
    } catch (err)
    {
        showMessage('product-type-message', 'Error saving product type: ' + err.message, 'error');
    }
}

async function editProductType(id)
{
    const t = await api(`/producttypes/${id}`);
    document.getElementById('product-type-id').value = t.id;
    document.getElementById('product-type-name').value = t.name;
    document.getElementById('product-type-description').value = t.description ?? '';
    document.getElementById('product-type-form-title').textContent = 'Edit Product Type';
}

async function deleteProductType(id)
{
    if (!confirm('Delete this product type? This will fail if products still reference it.')) return;

    try
    {
        await api(`/producttypes/${id}`, 'DELETE');
        await loadProductTypes();
        await loadProducts();
    } catch (err)
    {
        showMessage('product-type-message', err.message, 'error');
    }
}

function clearProductTypeForm()
{
    document.getElementById('product-type-id').value = '';
    document.getElementById('product-type-name').value = '';
    document.getElementById('product-type-description').value = '';
    document.getElementById('product-type-form-title').textContent = 'Add / Edit Product Type';
}

document.addEventListener('DOMContentLoaded', async () =>
{
    try
    {
        await fetchAntiforgeryToken();
        await loadProductTypes();
        await loadProducts();
    } catch (err)
    {
        showMessage('message', 'Unable to load catalog data: ' + err.message, 'error');
    }
});

// const API_BASE = '/catalog';

// // async function api(path, method = 'GET', body = null)
// // {
// //     const opts = {
// //         method,
// //         headers: { 'Content-Type': 'application/json', 'Accept': 'application/json' }
// //     };

// //     if (body) opts.body = JSON.stringify(body);

// //     const res = await fetch(`${API_BASE}${path}`, opts);
// //     if (!res.ok)
// //     {
// //         let message = `HTTP ${res.status}`;
// //         try
// //         {
// //             const errorBody = await res.json();
// //             if (errorBody?.message) message = errorBody.message;
// //         }
// //         catch { /* ignore non-json error bodies */ }
// //         throw new Error(message);
// //     }

// //     return res.status === 204 ? null : res.json();
// // }

// async function api(path, method = 'GET', body = null)
// {
//     const opts = { method, headers: { 'Content-Type': 'application/json', 'Accept': 'application/json' } };
//     if (body) opts.body = JSON.stringify(body);

//     const res = await fetch(`${API_BASE}${path}`, opts);

//     if (res.status === 401)
//     {
//         // Unauthorized – redirect to login
//         window.location.href = '/Account/Login';
//         throw new Error('Please log in.');
//     }
//     if (res.status === 403)
//     {
//         showMessage('message', 'You do not have permission to perform this action.', 'error');
//         throw new Error('Forbidden');
//     }

//     if (!res.ok)
//     {
//         let message = `HTTP ${res.status}`;
//         try { const errorBody = await res.json(); if (errorBody?.message) message = errorBody.message; }
//         catch { /* ignore */ }
//         throw new Error(message);
//     }

//     return res.status === 204 ? null : res.json();
// }

// function escHtml(str)
// {
//     return String(str ?? '').replace(/[&<>"']/g, c =>
//         ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[c]));
// }

// function showMessage(elementId, text, type)
// {
//     const element = document.getElementById(elementId);
//     element.textContent = text;
//     element.className = `message ${type}`;
//     setTimeout(() => { element.className = 'message'; }, 4000);
// }

// async function loadProductTypes()
// {
//     document.getElementById('product-types-loading').style.display = 'block';
//     document.getElementById('product-types-table').style.display = 'none';

//     const types = await api('/producttypes');
//     renderProductTypes(types);
//     populateProductTypeSelect(types);
// }

// function renderProductTypes(types)
// {
//     const tbody = document.getElementById('product-types-body');
//     tbody.innerHTML = types.length === 0
//         ? '<tr><td colspan="4" style="text-align:center;color:#999">No product types found.</td></tr>'
//         : types.map(t => `
//             <tr>
//                 <td><strong>${escHtml(t.name)}</strong></td>
//                 <td>${escHtml(t.description ?? '')}</td>
//                 <td>${t.productCount ?? 0}</td>
//                 <td>
//                     <button class="btn btn-edit" onclick="editProductType('${t.id}')">Edit</button>
//                     <button class="btn btn-danger" onclick="deleteProductType('${t.id}')">Delete</button>
//                 </td>
//             </tr>`).join('');

//     document.getElementById('product-types-loading').style.display = 'none';
//     document.getElementById('product-types-table').style.display = 'table';
// }

// function populateProductTypeSelect(types)
// {
//     const sel = document.getElementById('product-type');
//     const current = sel.value;
//     sel.innerHTML = '<option value="">Select type...</option>' +
//         types.map(t => `<option value="${t.id}">${escHtml(t.name)}</option>`).join('');
//     if (current) sel.value = current;
// }

// async function loadProducts()
// {
//     document.getElementById('loading').style.display = 'block';
//     document.getElementById('products-table').style.display = 'none';

//     const products = await api('/products');
//     renderProducts(products);
// }

// function renderProducts(products)
// {
//     const tbody = document.getElementById('products-body');
//     tbody.innerHTML = products.length === 0
//         ? '<tr><td colspan="5" style="text-align:center;color:#999">No products found.</td></tr>'
//         : products.map(p => `
//             <tr>
//                 <td><strong>${escHtml(p.name)}</strong></td>
//                 <td>${escHtml(p.productTypeName ?? '')}</td>
//                 <td>R ${Number(p.price).toFixed(2)}</td>
//                 <td>${escHtml(p.description ?? '')}</td>
//                 <td>
//                     <button class="btn btn-edit" onclick="editProduct('${p.id}')">Edit</button>
//                     <button class="btn btn-danger" onclick="deleteProduct('${p.id}')">Delete</button>
//                 </td>
//             </tr>`).join('');

//     document.getElementById('loading').style.display = 'none';
//     document.getElementById('products-table').style.display = 'table';
// }

// async function saveProduct()
// {
//     const id = document.getElementById('product-id').value;
//     const dto = {
//         name: document.getElementById('name').value.trim(),
//         price: parseFloat(document.getElementById('price').value),
//         productTypeId: document.getElementById('product-type').value,
//         description: document.getElementById('description').value.trim() || null
//     };

//     if (!dto.name || !dto.productTypeId || Number.isNaN(dto.price) || dto.price <= 0)
//     {
//         showMessage('message', 'Please complete all required product fields.', 'error');
//         return;
//     }

//     try
//     {
//         if (id) await api(`/products/${id}`, 'PUT', dto);
//         else await api('/products', 'POST', dto);

//         showMessage('message', 'Product saved successfully.', 'success');
//         clearForm();
//         await loadProducts();
//     }
//     catch (err)
//     {
//         showMessage('message', 'Error saving product: ' + err.message, 'error');
//     }
// }

// async function editProduct(id)
// {
//     const p = await api(`/products/${id}`);
//     document.getElementById('product-id').value = p.id;
//     document.getElementById('name').value = p.name;
//     document.getElementById('price').value = p.price;
//     document.getElementById('product-type').value = p.productTypeId;
//     document.getElementById('description').value = p.description ?? '';
//     document.getElementById('form-title').textContent = 'Edit Product';
// }

// async function deleteProduct(id)
// {
//     if (!confirm('Are you sure you want to delete this product?')) return;

//     try
//     {
//         await api(`/products/${id}`, 'DELETE');
//         await loadProducts();
//     }
//     catch (err)
//     {
//         showMessage('message', 'Error deleting product: ' + err.message, 'error');
//     }
// }

// function clearForm()
// {
//     document.getElementById('product-id').value = '';
//     document.getElementById('name').value = '';
//     document.getElementById('price').value = '';
//     document.getElementById('description').value = '';
//     document.getElementById('product-type').value = '';
//     document.getElementById('form-title').textContent = 'Add / Edit Product';
// }

// async function saveProductType()
// {
//     const id = document.getElementById('product-type-id').value;
//     const dto = {
//         name: document.getElementById('product-type-name').value.trim(),
//         description: document.getElementById('product-type-description').value.trim() || null
//     };

//     if (!dto.name)
//     {
//         showMessage('product-type-message', 'Product type name is required.', 'error');
//         return;
//     }

//     try
//     {
//         if (id) await api(`/producttypes/${id}`, 'PUT', dto);
//         else await api('/producttypes', 'POST', dto);

//         showMessage('product-type-message', 'Product type saved successfully.', 'success');
//         clearProductTypeForm();
//         await loadProductTypes();
//     }
//     catch (err)
//     {
//         showMessage('product-type-message', 'Error saving product type: ' + err.message, 'error');
//     }
// }

// async function editProductType(id)
// {
//     const t = await api(`/producttypes/${id}`);
//     document.getElementById('product-type-id').value = t.id;
//     document.getElementById('product-type-name').value = t.name;
//     document.getElementById('product-type-description').value = t.description ?? '';
//     document.getElementById('product-type-form-title').textContent = 'Edit Product Type';
// }

// async function deleteProductType(id)
// {
//     if (!confirm('Delete this product type? This will fail if products still reference it.')) return;

//     try
//     {
//         await api(`/producttypes/${id}`, 'DELETE');
//         await loadProductTypes();
//         await loadProducts();
//     }
//     catch (err)
//     {
//         showMessage('product-type-message', err.message, 'error');
//     }
// }

// function clearProductTypeForm()
// {
//     document.getElementById('product-type-id').value = '';
//     document.getElementById('product-type-name').value = '';
//     document.getElementById('product-type-description').value = '';
//     document.getElementById('product-type-form-title').textContent = 'Add / Edit Product Type';
// }



// document.addEventListener('DOMContentLoaded', async () =>
// {
//     try
//     {
//         await loadProductTypes();
//         await loadProducts();
//     }
//     catch (err)
//     {
//         showMessage('message', 'Unable to load catalog data: ' + err.message, 'error');
//     }
// });
