function toggleChildren(nodeId) {
    var childrenContainer = document.getElementById('children-' + nodeId);
    var toggleButton = childrenContainer.previousElementSibling.querySelector('.btn-secondary');

    if (childrenContainer.style.display === 'none') {
        childrenContainer.style.display = 'flex';
        toggleButton.innerHTML = 'Скрыть'; 
    } else {
        childrenContainer.style.display = 'none';
        toggleButton.innerHTML = 'Показать'; 
    }
}

function showAddChildForm(nodeId, parentOrder) {
    const formContainer = document.getElementById(`form-container`);
    formContainer.innerHTML = `
        <form onsubmit="createNode(event, ${nodeId}, ${parentOrder})">
            <input type="text" name="name" placeholder="Введите имя нового узла" required class="form-control" />
            <button type="submit" class="btn btn-success mt-2">Добавить</button>
            <button type="button" class="btn btn-secondary mt-2" onclick="clearForm()">Отмена</button>
        </form>`; //Форма для создания узла
}

function createNode(event, parentId, parentOrder) {
    event.preventDefault();
    const formData = new FormData(event.target);
    formData.append('parentId', parentId);
    formData.append('parentOrder', parentOrder);

    fetch('/Tree/CreateNode', {
        method: 'POST',
        body: new URLSearchParams(formData)
    }).then(response => location.reload());
}

function showMoveNodeForm(nodeId) {
    fetch(`/Tree/GetNodesForMove/${nodeId}`)
        .then(response => response.json())
        .then(nodes => {
            const formContainer = document.getElementById(`form-container`);

            if (nodes.length === 0) {
                formContainer.innerHTML = `
                    <div class="alert alert-warning" role="alert">
                        Нет доступных узлов для перемещения.
                    </div>
                    <button class="btn btn-secondary mt-2" type="button" onclick="clearForm()">Закрыть</button>`;
            } else {
                let options = nodes.map(node => `<option value="${node.id}">${node.name}</option>`).join('');
                formContainer.innerHTML = `
                    <form onsubmit="moveNode(event, ${nodeId})">
                        <select class="form-select" name="newParentId" required>${options}</select>
                        <button class="btn btn-success mt-2" type="submit">Переместить</button>
                        <button class="btn btn-secondary mt-2" type="button" onclick="clearForm()">Отмена</button>
                    </form>`;
            }
        })
        .catch(error => {
            console.error('Error fetching nodes:', error);
        });
}

function moveNode(event, nodeId) {
    event.preventDefault();
    const formData = new FormData(event.target);
    fetch(`/Tree/MoveNode/${nodeId}`, {
        method: 'POST',
        body: new URLSearchParams(formData)
    }).then(response => location.reload()); // Перезагрузка страницы для обновления дерева
}

function deleteNode(nodeId) {
    if (confirm("Вы уверены, что хотите удалить этот узел?")) {
        fetch(`/Tree/DeleteNode/${nodeId}`, { method: 'POST' })
            .then(response => location.reload());
    }
}

function clearForm() {
    const formContainer = document.getElementById(`form-container`);
    formContainer.innerHTML = '';
}

