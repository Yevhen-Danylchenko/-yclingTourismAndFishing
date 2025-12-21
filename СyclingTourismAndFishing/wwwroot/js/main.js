// Р”Р»СЏ РіРѕР»РѕРІРЅРѕС— СЃС‚РѕСЂС–РЅРєРё
if (document.getElementById('categories-container')) {
    loadCategories();
    loadMenuItems();
}

// Р”Р»СЏ Р°РґРјС–РЅ-РїР°РЅРµР»С–
if (document.getElementById('admin-menu-container')) {
    loadAdminMenuItems();
}

// Р”Р»СЏ СЃС‚РѕСЂС–РЅРєРё РґРѕРґР°РІР°РЅРЅСЏ СЃС‚СЂР°РІРё
if (document.getElementById('categorySelect') && window.location.pathname.includes('add-item')) {
    loadCategoriesForSelect();
}

// Р”Р»СЏ СЃС‚РѕСЂС–РЅРєРё СЂРµРґР°РіСѓРІР°РЅРЅСЏ
if (document.getElementById('categorySelect') && window.location.pathname.includes('edit-item')) {
    loadCategoriesForSelect();
    loadItemForEdit();
}

if (document.getElementById('user-menu-container')) {
    loadUserItems();
}

async function loadCategories() {
    const response = await fetch('/api/categories');
    const categories = await response.json();

    const container = document.getElementById('categories-container');
    container.innerHTML = categories.map(c => `
        <div class="category-card">
            <img src="${c.imageUrl}" alt="${c.name}">
            <h3>${c.name}</h3>
            <p>${c.description}</p>
        </div>
    `).join('');
}

async function loadMenuItems() {
    const response = await fetch('/api/menu-items');
    const menuItems = await response.json();

    const container = document.getElementById('menu-container');
    container.innerHTML = menuItems.map(m => `
        <div class="menu-card">
            <img src="${m.imageUrl}" alt="${m.name}">
            <div class="menu-card-content">
                <h3>${m.name}</h3>
                <p>${m.description}</p>
                <div class="price">${m.price} ₴</div>
            </div>
        </div>
    `).join('');
}

async function loadAdminMenuItems() {
    const response = await fetch('/api/admin/menu-items');
    const menuItems = await response.json();

    const container = document.getElementById('admin-menu-container');
    container.innerHTML = menuItems.map(m => `
        <div class="menu-card">
            <img src="${m.imageUrl}" alt="${m.name}">
            <div class="menu-card-content">
                <h3>${m.name}</h3>
                <p>${m.description}</p>
                <div class="price">${m.price} ₴</div>
                <div class="admin-actions">
                    <button class="btn-edit" onclick="location.href='/admin/edit-item/${m.id}'">Р РµРґР°РіСѓРІР°С‚Рё</button>
                    <form method="post" action="/admin/delete-item/${m.id}" style="display: inline;">
                        <button type="submit" class="btn-delete" onclick="return confirm('Р’РёРґР°Р»РёС‚Рё С†СЋ СЃС‚СЂР°РІСѓ?')">Р’РёРґР°Р»РёС‚Рё</button>
                    </form>
                </div>
            </div>
        </div>
    `).join('');
}

async function loadCategoriesForSelect() {
    const response = await fetch('/api/categories');
    const categories = await response.json();

    const select = document.getElementById('categorySelect');
    select.innerHTML = categories.map(c =>
        `<option value="${c.id}">${c.name}</option>`
    ).join('');
}

async function loadItemForEdit() {
    const id = window.location.pathname.split('/').pop();
    const response = await fetch(`/api/menu-items/${id}`);
    const item = await response.json();

    document.getElementById('itemId').value = item.id;
    document.getElementById('itemName').value = item.name;
    document.getElementById('itemDescription').value = item.description;
    document.getElementById('itemPrice').value = item.price;
    document.getElementById('itemImageUrl').value = item.imageUrl;
    document.getElementById('categorySelect').value = item.categoryId;

    document.getElementById('editForm').action = `/admin/edit-item/${id}`;
}

async function loadUserMenuItems() {
    const response = await fetch('/api/user-panel/menu-items');
    const menuItems = await response.json();

    const container = document.getElementById('user-menu-container');
    container.innerHTML = menuItems.map(m => `
        <div class="menu-card">
            <img src="${m.imageUrl}" alt="${m.name}">
            <div class="menu-card-content">
                <h3>${m.name}</h3>
                <p>${m.description}</p>
                <div class="price">${m.price} ₴</div>
                <div class="user-actions">
                    <button class="btn-buy" onclick="addToCart(${m.id})">Придбати</button>
                </div>
            </div>
        </div>
    `).join('');
}

async function loadUserItems() {
    try {
        const response = await fetch('/api/user-panel/items');
        if (!response.ok) {
            throw new Error("Не вдалося завантажити товари");
        }

        const items = await response.json();
        const container = document.getElementById('user-menu-container');

        container.innerHTML = items.map(i => `
            <div class="menu-card">
                <img src="${i.imageUrl}" alt="${i.name}">
                <div class="menu-card-content">
                    <h3>${i.name}</h3>
                    <p>${i.description}</p>
                    <div class="price">${i.price} ₴</div>
                    <div class="user-actions">
                        <button class="btn-buy" onclick="buyItem(${i.id})">Придбати</button>
                    </div>
                </div>
            </div>
        `).join('');
    } catch (err) {
        console.error(err);
        document.getElementById('user-menu-container').innerHTML =
            "<p style='color:red'>Помилка завантаження товарів</p>";
    }
}

// Викликати при завантаженні сторінки
document.addEventListener("DOMContentLoaded", loadUserItems);

// Функція покупки
//async function buyItem(itemId) {
//    try {
//        const response = await fetch('/api/cart/add', {
//            method: 'POST',
//            headers: { 'Content-Type': 'application/json' },
//            body: JSON.stringify({ itemId: itemId, quantity: 1 })
//        });

//        if (response.ok) {
//            alert("Товар додано до кошика!");
//        } else {
//            const error = await response.text();
//            alert("Помилка: " + error);
//        }
//    } catch (err) {
//        alert("Сталася помилка: " + err.message);
//    }
//}

// функція для покупки
async function buyItem(itemId) {
    try {
        const response = await fetch('/api/cart/add', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ ItemId: itemId, quantity: 1 })
        });

        if (response.ok) {
            alert("Товар додано до кошика!");
        } else {
            const error = await response.text();
            alert("Помилка: " + error);
        }
    } catch (err) {
        alert("Сталася помилка при додаванні товару: " + err.message);
    }
}

function goToUserPanel() {
    // редірект на сторінку Юзер-панелі
    window.location.href = "/user-panel";
}

// приклад використання: підключаємо до форми
document.addEventListener("DOMContentLoaded", () => {
    const loginForm = document.querySelector("form");

    if (loginForm) {
        loginForm.addEventListener("submit", function (e) {
            e.preventDefault(); // блокуємо стандартну відправку
            // тут можна додати перевірку логіну/пароля
            goToUserPanel(); // виклик функції переходу
        });
    }
});

