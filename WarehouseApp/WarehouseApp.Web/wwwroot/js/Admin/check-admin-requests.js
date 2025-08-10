let lastPendingCount = 0;

async function checkAdminRequests() {
    try {
        const response = await fetch('/Admin/UserManagement/CheckPendingAdminRequests');
        const data = await response.json();
        const badge = document.getElementById('admin-requests-badge');

        if (data.hasPending) {
            badge.textContent = data.count;
            badge.style.display = 'inline-block';

            // Flash effect if count increased
            if (data.count > lastPendingCount && lastPendingCount !== 0) {
                badge.classList.add('flash-badge');
                setTimeout(() => badge.classList.remove('flash-badge'), 5000);
            }

            lastPendingCount = data.count;
        } else {
            badge.style.display = 'none';
            lastPendingCount = 0;
        }
    } catch (err) {
        console.error('Error fetching admin request status:', err);
    }
}

checkAdminRequests();

setInterval(checkAdminRequests, 60000);
