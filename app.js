// 🔧 CONFIGURACIÓN PARA EL DESPLIEGUE (RENDER)
// ----------------------------------------------------
// CUANDO TENGAS EL LINK DE RENDER, REEMPLAZA EL LINK DE LOCALHOST POR EL DE RENDER.
// EJEMPLO: const API_BASE_URL = 'https://skillvault-api.onrender.com';
const API_BASE_URL = ''; // Relative path because Nginx reverse proxy will handle /api/ routing
// ----------------------------------------------------

// QOL: Toast Notifications & Theme Toggle & PWA Registration
window.showToast = function(message, type = 'info') {
    const container = document.getElementById('toast-container');
    if (!container) return;
    
    const toast = document.createElement('div');
    toast.className = `toast ${type}`;
    
    let icon = 'info';
    if (type === 'success') icon = 'check-circle';
    if (type === 'error') icon = 'alert-circle';
    if (type === 'warning') icon = 'alert-triangle';
    
    toast.innerHTML = `<i data-lucide="${icon}"></i> <span>${message}</span>`;
    container.appendChild(toast);
    
    if (window.lucide) {
        window.lucide.createIcons({ root: toast });
    }
    
    setTimeout(() => toast.classList.add('show'), 10);
    setTimeout(() => {
        toast.classList.remove('show');
        setTimeout(() => toast.remove(), 300);
    }, 3000);
};

// Override window.alert
window.alert = function(message) {
    let type = 'warning';
    if (typeof message === 'string') {
        const lowerMsg = message.toLowerCase();
        if (lowerMsg.includes('success') || lowerMsg.includes('sync established') || lowerMsg.includes('logged')) {
            type = 'success';
        } else if (lowerMsg.includes('failed') || lowerMsg.includes('error') || lowerMsg.includes('invalid')) {
            type = 'error';
        }
    }
    showToast(message, type);
};

// PWA Service Worker Registration
if ('serviceWorker' in navigator) {
  window.addEventListener('load', () => {
    navigator.serviceWorker.register('/service-worker.js')
      .then(registration => console.log('SW registered'))
      .catch(err => console.log('SW registration failed: ', err));
  });
}

// Dark/Light Theme Initialization
const savedTheme = localStorage.getItem('skillvault-theme') || 'dark';
if (savedTheme === 'light') {
    document.body.classList.add('light-mode');
}

document.addEventListener('DOMContentLoaded', () => {
    const btnThemeToggle = document.getElementById('btnThemeToggle');
    if (btnThemeToggle) {
        // Initial icon state
        const iconElement = btnThemeToggle.querySelector('i');
        const textElement = btnThemeToggle.querySelector('span');
        if (savedTheme === 'light') {
            if (iconElement) iconElement.setAttribute('data-lucide', 'sun');
            if (textElement) textElement.innerText = 'Modo Oscuro';
        }
        
        btnThemeToggle.addEventListener('click', () => {
            const isLight = document.body.classList.toggle('light-mode');
            localStorage.setItem('skillvault-theme', isLight ? 'light' : 'dark');
            
            // Update icon and text
            if (iconElement) iconElement.setAttribute('data-lucide', isLight ? 'sun' : 'moon');
            if (textElement) textElement.innerText = isLight ? 'Modo Oscuro' : 'Modo Claro';
            
            if (window.lucide) window.lucide.createIcons();
        });
    }
});

// High-Agency Application State Sourcing
let appState = {
    isLive: false,
    apiUrl: API_BASE_URL,
    isAuthenticated: false,
    authToken: null,
    skills: [],
    certifications: [],
    courses: [],
    progressEntries: [],
    progressSummary: {
        totalCertifications: 0,
        completedCertifications: 0,
        inProgressCertifications: 0,
        totalHoursSpent: 0.0,
        certificationProgress: []
    }
};

// Organic Mock Data for visual calibration
const mockSkills = [
    { id: 1, name: "Cloud Computing", description: "Design and deployment of AWS architectures utilizing EC2, RDS, IAM, S3, and secure VPC routing.", level: "Intermediate", targetHours: 150, certificationCount: 2, createdAt: new Date(), updatedAt: new Date() },
    { id: 2, name: "Desarrollo Backend .NET", description: "Clean Architecture in C# 12, ASP.NET Core controllers, EF Core mapping, and database schema updates.", level: "Advanced", targetHours: 200, certificationCount: 1, createdAt: new Date(), updatedAt: new Date() },
    { id: 3, name: "PostgreSQL DBA", description: "DDL migrations, transactional index optimization, query execution profiling, and partition setups.", level: "Intermediate", targetHours: 80, certificationCount: 1, createdAt: new Date(), updatedAt: new Date() }
];

const mockCerts = [
    { id: 1, title: "AWS Certified Cloud Practitioner", provider: "Amazon Web Services", completedDate: "2026-05-15", credentialUrl: "https://aws.amazon.com", skillIds: [1], createdAt: new Date(), updatedAt: new Date() },
    { id: 2, title: "ASP.NET Core Web API Fundamentos", provider: "Pluralsight", completedDate: "2026-06-10", credentialUrl: "https://pluralsight.com", skillIds: [2], createdAt: new Date(), updatedAt: new Date() },
    { id: 3, title: "PostgreSQL Database Administrator", provider: "LinkedIn Learning", completedDate: "2026-06-25", credentialUrl: "https://linkedin.com", skillIds: [3], createdAt: new Date(), updatedAt: new Date() }
];

const mockCourses = [
    { id: 1, title: "Advanced Entity Framework Core", provider: "Pluralsight", status: "InProgress", url: "https://pluralsight.com", createdAt: new Date() },
    { id: 2, title: "Microservices with Node JS and React", provider: "Udemy", status: "InProgress", url: "https://udemy.com", createdAt: new Date() }
];

const mockProgress = [
    { id: 1, certificationId: 1, skillId: 1, hours: 14.5, notes: "Configured public/private subnets and route tables inside AWS VPC.", recordedAt: "2026-06-24T18:30:00.000Z" },
    { id: 2, certificationId: 2, skillId: 2, hours: 8.5, notes: "Wired PostgreSQL DbContext and configured migrations inside Program.cs.", recordedAt: "2026-06-25T14:20:00.000Z" },
    { id: 3, certificationId: 3, skillId: 3, hours: 5.0, notes: "Analyzed slow queries and implemented database indexes for foreign keys.", recordedAt: "2026-06-26T09:15:00.000Z" },
    { id: 4, courseId: 1, skillId: null, hours: 2.5, notes: "Learned about EF Core compiled models.", recordedAt: "2026-06-26T14:15:00.000Z" }
];

// Active Chart instance
let hoursChartInstance = null;

// Initializer Hook
document.addEventListener("DOMContentLoaded", () => {
    setupNavigation();
    setupForms();
    setupAPIConnection();
    
    const btnLogout = document.getElementById("btnLogout");
    if (btnLogout) {
        btnLogout.addEventListener("click", () => {
            localStorage.removeItem("skillvault_jwt_token");
            appState.authToken = null;
            appState.isAuthenticated = false;
            showLoginUI(true);
        });
    }
    
    // Attempt local API connection. Fall back quietly to mock on refusal.
    tryConnect(false);
});

// Sidebar & Tabs Navigation
function setupNavigation() {
    const navButtons = document.querySelectorAll(".menu-item");
    const tabViews = document.querySelectorAll(".tab-view");
    const pageTitle = document.getElementById("pageTitle");
    const pageSubtitle = document.getElementById("pageSubtitle");

    const tabDetails = {
        dashboard: { title: "Ledger Summary", subtitle: "A centralized index of courses, achievements, and active learning time." },
        skills: { title: "Mapped Skills", subtitle: "Strategic capability domains and hour goals." },
        certifications: { title: "Tracked Credentials", subtitle: "Certifications and verified platform credentials." },
        courses: { title: "Active Courses", subtitle: "Courses currently in progress." },
        progress: { title: "Session Recorder", subtitle: "Log learning sessions and study details to database." }
    };

    navButtons.forEach(btn => {
        btn.addEventListener("click", () => {
            const tabId = btn.getAttribute("data-tab");
            
            navButtons.forEach(b => b.classList.remove("active"));
            tabViews.forEach(t => t.classList.remove("active"));
            
            btn.classList.add("active");
            document.getElementById(`tab-${tabId}`).classList.add("active");

            // Update Page Details
            pageTitle.textContent = tabDetails[tabId].title;
            pageSubtitle.textContent = tabDetails[tabId].subtitle;
        });
    });

    // Custom slider value update
    const rangeInput = document.getElementById("progressHours");
    const rangeVal = document.getElementById("hoursVal");
    rangeInput.addEventListener("input", (e) => {
        rangeVal.textContent = `${parseFloat(e.target.value).toFixed(1)} hrs`;
    });
}

// Modal Controllers
function openModal(id) {
    document.getElementById(id).classList.add("active");
}

function closeModal(id) {
    document.getElementById(id).classList.remove("active");
}

class ApiError extends Error {
    constructor(message, status, payload = null) {
        super(message);
        this.name = "ApiError";
        this.status = status;
        this.payload = payload;
    }
}

const sleep = (ms) => new Promise((r) => setTimeout(r, ms));

const fetchWithBackoff = async (fn, retries = 3, delay = 300) => {
    try {
        return await fn();
    } catch (err) {
        const isAbort = err.name === "AbortError";
        const isHttpError = typeof err.status === "number";
        const isRetryable = !isAbort && (!isHttpError || err.status >= 500);

        if (retries <= 0 || !isRetryable) throw err;

        const nextDelay = delay * 2 + Math.random() * 100;
        await sleep(nextDelay);

        return fetchWithBackoff(fn, retries - 1, nextDelay);
    }
};

async function authenticatedFetch(url, options = {}) {
    options.headers = options.headers || {};
    if (appState.authToken) {
        options.headers['Authorization'] = `Bearer ${appState.authToken}`;
    }
    options.headers['Content-Type'] = options.headers['Content-Type'] || 'application/json';

    return fetchWithBackoff(async () => {
        const res = await fetch(url, options);

        if (!res.ok) {
            let payload = null;
            try {
                payload = await res.json();
            } catch (_) {}

            if (res.status === 401) {
                // Token might be expired, trigger logout
                localStorage.removeItem("skillvault_jwt_token");
                appState.authToken = null;
                appState.isAuthenticated = false;
                showLoginUI(true);
            }

            throw new ApiError(
                payload?.message || "Request failed",
                res.status,
                payload
            );
        }

        if (res.status === 204) return null;

        const text = await res.text();
        return text ? JSON.parse(text) : null;
    });
}

function setupAuthFlow() {
    const savedToken = localStorage.getItem("skillvault_jwt_token");
    if (savedToken && isTokenValid(savedToken)) {
        appState.authToken = savedToken;
        appState.isAuthenticated = true;
        showLoginUI(false);
    } else {
        appState.authToken = null;
        appState.isAuthenticated = false;
    }
}

function isTokenValid(token) {
    try {
        const payload = JSON.parse(atob(token.split('.')[1]));
        return payload.exp * 1000 > Date.now();
    } catch {
        return false;
    }
}

function showLoginUI(show) {
    const loginModal = document.getElementById("loginModal") || createLoginModal();
    loginModal.style.display = show ? "flex" : "none";
    if (show) {
        document.body.classList.remove("authenticated");
    } else {
        document.body.classList.add("authenticated");
    }
}

function createLoginModal() {
    const modal = document.createElement("div");
    modal.id = "loginModal";
    modal.className = "login-modal-overlay";
    modal.innerHTML = `
        <div class="login-modal">
            <h2>SkillVault</h2>
            <div class="auth-tabs">
                <button type="button" class="auth-tab active" id="tabLoginBtn">Log In</button>
                <button type="button" class="auth-tab" id="tabRegisterBtn">Register</button>
            </div>
            
            <form id="loginForm" class="active">
                <div class="login-modal-group">
                    <label for="loginEmail">Email Address</label>
                    <input type="email" id="loginEmail" placeholder="e.g., developer@skillvault.dev" required>
                </div>
                <div class="login-modal-group">
                    <label for="loginPassword">Password</label>
                    <input type="password" id="loginPassword" placeholder="Enter your password" required>
                </div>
                <button type="submit">Login</button>
            </form>

            <form id="registerForm">
                <div class="login-modal-group">
                    <label for="registerEmail">Email Address</label>
                    <input type="email" id="registerEmail" placeholder="e.g., newuser@skillvault.dev" required>
                </div>
                <div class="login-modal-group">
                    <label for="registerPassword">Password</label>
                    <input type="password" id="registerPassword" placeholder="Create a password" required>
                </div>
                <div class="login-modal-group">
                    <label for="registerConfirmPassword">Confirm Password</label>
                    <input type="password" id="registerConfirmPassword" placeholder="Confirm your password" required>
                </div>
                <button type="submit">Create Account</button>
            </form>
        </div>
    `;
    document.body.appendChild(modal);

    const tabLoginBtn = modal.querySelector("#tabLoginBtn");
    const tabRegisterBtn = modal.querySelector("#tabRegisterBtn");
    const loginForm = modal.querySelector("#loginForm");
    const registerForm = modal.querySelector("#registerForm");

    tabLoginBtn.addEventListener("click", () => {
        tabLoginBtn.classList.add("active");
        tabRegisterBtn.classList.remove("active");
        loginForm.classList.add("active");
        registerForm.classList.remove("active");
    });

    tabRegisterBtn.addEventListener("click", () => {
        tabRegisterBtn.classList.add("active");
        tabLoginBtn.classList.remove("active");
        registerForm.classList.add("active");
        loginForm.classList.remove("active");
    });

    loginForm.addEventListener("submit", handleLogin);
    registerForm.addEventListener("submit", handleRegister);
    return modal;
}

async function handleLogin(e) {
    e.preventDefault();
    const email = document.getElementById("loginEmail").value.trim();
    const password = document.getElementById("loginPassword").value;

    if (appState.isLive) {
        try {
            const response = await fetch(`${appState.apiUrl}/api/v1/auth/login`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ email, password })
            });

            if (response.ok) {
                const data = await response.json();
                appState.authToken = data.accessToken;
                appState.isAuthenticated = true;
                localStorage.setItem("skillvault_jwt_token", data.accessToken);
                showLoginUI(false);
                
                await loadDataFromApi();
                renderAll();
            } else {
                alert("Invalid credentials. Try jj@skillvault.dev / accenture2026");
            }
        } catch (error) {
            alert("Login failed: " + error.message);
        }
    } else {
        const mockUsers = JSON.parse(localStorage.getItem("skillvault_mock_users") || "[]");
        const matchesMock = mockUsers.some(u => u.email === email && u.password === password);
        const matchesHardcoded = (email === "jj@skillvault.dev" && password === "accenture2026");

        if (matchesMock || matchesHardcoded) {
            appState.isAuthenticated = true;
            appState.authToken = "mock-offline-token";
            showLoginUI(false);
            loadMockData();
            renderAll();
        } else {
            alert("Invalid offline credentials. Register an account first.");
        }
    }
}

async function handleRegister(e) {
    e.preventDefault();
    const email = document.getElementById("registerEmail").value.trim();
    const password = document.getElementById("registerPassword").value;
    const confirmPassword = document.getElementById("registerConfirmPassword").value;

    if (password !== confirmPassword) {
        alert("Passwords do not match.");
        return;
    }

    if (appState.isLive) {
        try {
            const response = await fetch(`${appState.apiUrl}/api/v1/auth/register`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ email, password })
            });

            if (response.ok) {
                alert("Account created successfully! Please log in.");
                document.getElementById("tabLoginBtn").click();
                document.getElementById("loginEmail").value = email;
                document.getElementById("loginPassword").value = password;
            } else {
                const err = await response.json();
                alert(`Registration failed: ${err.message || 'Unknown error'}`);
            }
        } catch (error) {
            alert("Registration failed: " + error.message);
        }
    } else {
        let mockUsers = JSON.parse(localStorage.getItem("skillvault_mock_users") || "[]");
        if (mockUsers.some(u => u.email === email)) {
            alert("User already exists.");
            return;
        }
        mockUsers.push({ email, password });
        localStorage.setItem("skillvault_mock_users", JSON.stringify(mockUsers));
        alert("Account registered locally (Offline Mode)! Please log in.");
        
        document.getElementById("tabLoginBtn").click();
        document.getElementById("loginEmail").value = email;
        document.getElementById("loginPassword").value = password;
    }
}

// API Server Setup
function setupAPIConnection() {
    const urlInput = document.getElementById("apiUrlInput");
    const btnConnect = document.getElementById("btnConnectApi");

    const savedUrl = localStorage.getItem("skillvault_api_url");
    if (savedUrl) {
        appState.apiUrl = savedUrl;
    }

    urlInput.value = appState.apiUrl;

    btnConnect.addEventListener("click", () => {
        appState.apiUrl = urlInput.value.trim();
        localStorage.setItem("skillvault_api_url", appState.apiUrl);
        const icon = btnConnect.querySelector("i, svg");
        if (icon) icon.classList.add("spin");
        tryConnect(true);
    });
}

// Test Connection & State Switcher
async function tryConnect(showAlert = false) {
    const badge = document.getElementById("connectionBadge");
    const badgeText = document.getElementById("connectionText");

    setupAuthFlow();

    try {
        const response = await fetch(`${appState.apiUrl}/health`, {
            method: 'GET',
            headers: { 'Accept': 'application/json' }
        });

        if (response.ok) {
            appState.isLive = true;
            badge.className = "connection-status-pill online";
            badgeText.textContent = "DATABASE ONLINE";
            if (showAlert) alert("Sync established with PostgreSQL database.");
            
            if (!appState.isAuthenticated) {
                showLoginUI(true);
                return;
            }
            
            await loadDataFromApi();
        } else {
            throw new Error("API online but database unreachable");
        }
    } catch (error) {
        console.warn("REST API offline. Loading organic local mocks.");
        appState.isLive = false;
        badge.className = "connection-status-pill offline";
        badgeText.textContent = "DEMO MODE";
        if (showAlert) alert("API Server unreachable. Operating in Offline Demo Mode.");
        
        if (!appState.isAuthenticated) {
            showLoginUI(true);
            return;
        }
        loadMockData();
    } finally {
        const btnConnectIcon = document.querySelector("#btnConnectApi i, #btnConnectApi svg");
        if (btnConnectIcon) btnConnectIcon.classList.remove("spin");
        renderAll();
    }
}

// Load Mock details
function loadMockData() {
    appState.skills = [...mockSkills];
    appState.certifications = [...mockCerts];
    appState.courses = [...mockCourses];
    appState.progressEntries = [...mockProgress];
    
    const totalHours = appState.progressEntries.reduce((sum, p) => sum + parseFloat(p.hours), 0.0);
    const certProgress = appState.certifications.map(cert => {
        return {
            certificationId: cert.id,
            title: cert.title,
            hoursSpent: appState.progressEntries.filter(p => p.certificationId === cert.id).reduce((sum, p) => sum + p.hours, 0.0)
        };
    });

    appState.progressSummary = {
        totalCertifications: appState.certifications.length,
        completedCertifications: appState.certifications.filter(c => new Date(c.completedDate) <= new Date()).length,
        inProgressCertifications: appState.certifications.filter(c => new Date(c.completedDate) > new Date()).length,
        totalHoursSpent: totalHours,
        certificationProgress: certProgress
    };
}

// Sync from backend REST endpoints
async function loadDataFromApi() {
    try {
        // Fetch Skills
        appState.skills = await authenticatedFetch(`${appState.apiUrl}/api/v1/skills`);

        // Fetch Certifications
        appState.certifications = await authenticatedFetch(`${appState.apiUrl}/api/v1/certifications`);

        // Fetch Courses
        try {
            appState.courses = await authenticatedFetch(`${appState.apiUrl}/api/v1/courses`);
        } catch (e) {
            console.warn("Courses API not available yet.", e);
            appState.courses = [];
        }

        // Fetch Progress Summary
        appState.progressSummary = await authenticatedFetch(`${appState.apiUrl}/api/v1/progress`);

        // Fetch detailed progress logs per certification
        let loadedLogs = [];
        for (const cert of appState.certifications) {
            try {
                const logs = await authenticatedFetch(`${appState.apiUrl}/api/v1/progress/certification/${cert.id}`);
                if (logs) loadedLogs = loadedLogs.concat(logs);
            } catch (e) {
                console.warn(`Failed fetching logs for cert ${cert.id}`, e);
            }
        }

        // Fetch detailed progress logs per course
        if (appState.courses) {
            for (const course of appState.courses) {
                try {
                    const logs = await authenticatedFetch(`${appState.apiUrl}/api/v1/progress/course/${course.id}`);
                    if (logs) loadedLogs = loadedLogs.concat(logs);
                } catch (e) {
                    console.warn(`Failed fetching logs for course ${course.id}`, e);
                }
            }
        }
        
        // Sort descending
        loadedLogs.sort((a,b) => new Date(b.recordedAt) - new Date(a.recordedAt));
        appState.progressEntries = loadedLogs;

    } catch (e) {
        console.error("Sync error:", e);
        alert("Failed to load records from database.");
    }
}

function renderAll() {
    renderDashboard();
    renderSkillsGrid();
    renderCertificationsTable();
    renderCoursesTable();
    populateDropdowns();
    lucide.createIcons();
}

// Dashboard View
function renderDashboard() {
    // Large metrics updates
    document.getElementById("statTotalCerts").textContent = appState.progressSummary.totalCertifications;
    document.getElementById("statTotalHours").textContent = parseFloat(appState.progressSummary.totalHoursSpent).toFixed(1);
    document.getElementById("statTotalSkills").textContent = appState.skills.length;
    document.getElementById("statCompletedCerts").textContent = appState.progressSummary.completedCertifications;

    // Session log timeline
    const timeline = document.getElementById("recentProgressList");
    timeline.innerHTML = "";

    if (appState.progressEntries.length === 0) {
        timeline.innerHTML = `<div class="timeline-empty-state">No sessions recorded. Connect API or add progress.</div>`;
    } else {
        appState.progressEntries.slice(0, 10).forEach(entry => {
            const cert = appState.certifications.find(c => c.id === entry.certificationId);
            const course = appState.courses ? appState.courses.find(c => c.id === entry.courseId) : null;
            const skill = appState.skills.find(s => s.id === entry.skillId);
            const time = new Date(entry.recordedAt).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
            const date = new Date(entry.recordedAt).toLocaleDateString([], { month: 'short', day: 'numeric' });

            let displayTitle = 'Progress Log';
            if (cert) displayTitle = cert.title;
            else if (course) displayTitle = course.title;
            else if (entry.certificationId) displayTitle = 'Credential #' + entry.certificationId;
            else if (entry.courseId) displayTitle = 'Course #' + entry.courseId;

            const auditItem = document.createElement("div");
            auditItem.className = "audit-item";
            auditItem.innerHTML = `
                <div class="audit-header">
                    <span class="audit-title">${displayTitle}</span>
                    <span class="audit-time">${date} &middot; ${time}</span>
                </div>
                <div class="audit-meta-row">
                    <span class="audit-hours font-mono">+${parseFloat(entry.hours).toFixed(1)} hrs</span>
                    <span class="desc-text">${skill ? skill.name : ''}</span>
                </div>
                <div class="audit-notes" title="${entry.notes || ''}">${entry.notes || 'No session notes recorded.'}</div>
            `;
            timeline.appendChild(auditItem);
        });
    }

    // Chart.js Canvas Update
    const canvas = document.getElementById("hoursChart");
    if (!canvas) return;

    const chartLabels = appState.progressSummary.certificationProgress.map(cp => cp.title.length > 25 ? cp.title.substring(0, 22) + "..." : cp.title);
    const chartData = appState.progressSummary.certificationProgress.map(cp => cp.hoursSpent);

    if (hoursChartInstance) {
        hoursChartInstance.destroy();
    }

    if (chartLabels.length === 0) {
        const ctx = canvas.getContext('2d');
        ctx.clearRect(0, 0, canvas.width, canvas.height);
        ctx.fillStyle = '#64748b';
        ctx.font = '13px Outfit';
        ctx.textAlign = 'center';
        ctx.fillText('No time data logged for credentials', canvas.width / 2, canvas.height / 2);
        return;
    }

    // Electric Blue Theme Colors
    hoursChartInstance = new Chart(canvas, {
        type: 'bar',
        data: {
            labels: chartLabels,
            datasets: [{
                data: chartData,
                backgroundColor: 'rgba(53, 104, 255, 0.15)',
                borderColor: '#3568ff',
                borderWidth: 1.5,
                borderRadius: 8,
                hoverBackgroundColor: 'rgba(53, 104, 255, 0.35)',
                hoverBorderColor: '#60a5fa'
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: { display: false }
            },
            scales: {
                y: {
                    grid: { color: 'rgba(255, 255, 255, 0.03)' },
                    ticks: { color: '#64748b', font: { family: 'Outfit', size: 11 } }
                },
                x: {
                    grid: { display: false },
                    ticks: { color: '#64748b', font: { family: 'Outfit', size: 10 } }
                }
            }
        }
    });
}

// Skills Grid
function renderSkillsGrid() {
    const grid = document.getElementById("skillsGrid");
    grid.innerHTML = "";

    if (appState.skills.length === 0) {
        grid.innerHTML = `<div class="double-bezel col-span-12" style="grid-column: 1/-1;"><div class="inner-core bento-card-padding text-center"><p class="subtitle">No skill domains mapped. Add a skill to configure time thresholds.</p></div></div>`;
        return;
    }

    appState.skills.forEach(skill => {
        const progressHours = appState.progressEntries
            .filter(p => p.skillId === skill.id)
            .reduce((sum, p) => sum + parseFloat(p.hours), 0.0);
            
        const percentage = Math.min(Math.round((progressHours * 100) / (skill.targetHours || 100)), 100);

        // Dynamically compute associated certification count from logs
        const associatedCertIds = new Set();
        appState.progressEntries
            .filter(p => p.skillId === skill.id)
            .forEach(p => associatedCertIds.add(p.certificationId));
        appState.certifications.forEach(c => {
            if (c.skillIds && c.skillIds.includes(skill.id)) {
                associatedCertIds.add(c.id);
            }
        });
        const certCount = associatedCertIds.size;

        const card = document.createElement("div");
        card.className = "skill-card-double-bezel";
        card.innerHTML = `
            <div class="inner-core">
                <div class="skill-card-header">
                    <span class="skill-card-title">${skill.name}</span>
                    <span class="level-pill-badge level-${skill.level.toLowerCase()}">${skill.level}</span>
                </div>
                <p class="skill-card-desc">${skill.description}</p>
                <div class="skill-card-progress">
                    <div class="progress-labels-row">
                        <span>Threshold: ${skill.targetHours} hrs</span>
                        <span class="progress-percent-indicator">${percentage}%</span>
                    </div>
                    <div class="progress-bar-groove">
                        <div class="progress-bar-strip" style="width: ${percentage}%"></div>
                    </div>
                </div>
                <div class="skill-card-footer">
                    <span class="skill-footer-metric"><i data-lucide="award"></i> ${certCount} Certs</span>
                    <span class="skill-footer-metric"><i data-lucide="clock"></i> ${progressHours.toFixed(1)} hrs</span>
                    <button class="skill-trash-btn" onclick="deleteSkill(${skill.id})" title="Delete Skill">
                        <i data-lucide="trash-2"></i>
                    </button>
                </div>
            </div>
        `;
        grid.appendChild(card);
    });
}

// Credentials Table
function renderCertificationsTable() {
    const tbody = document.getElementById("certsTableBody");
    tbody.innerHTML = "";

    if (appState.certifications.length === 0) {
        tbody.innerHTML = `<tr><td colspan="6" class="timeline-empty-state">No credentials registered.</td></tr>`;
        return;
    }

    appState.certifications.forEach(cert => {
        // Dynamically compute associated skill tags from explicit links AND progress entries
        const associatedSkillIds = new Set(cert.skillIds || []);
        appState.progressEntries
            .filter(p => p.certificationId === cert.id && p.skillId)
            .forEach(p => associatedSkillIds.add(p.skillId));

        const chipsList = Array.from(associatedSkillIds).map(id => {
            const sk = appState.skills.find(s => s.id === id);
            return sk ? `<span class="tag-chip">${sk.name}</span>` : '';
        }).join('');

        const row = document.createElement("tr");
        row.innerHTML = `
            <td class="cert-title-cell">${cert.title}</td>
            <td><span class="provider-capsule">${cert.provider}</span></td>
            <td class="date-mono">${new Date(cert.completedDate).toLocaleDateString([], { year: 'numeric', month: 'numeric', day: 'numeric' })}</td>
            <td>
                ${cert.credentialUrl ? `<a href="${cert.credentialUrl}" target="_blank" class="table-link-btn">Verify <i data-lucide="arrow-up-right"></i></a>` : '<span class="subtitle">—</span>'}
            </td>
            <td>
                <div class="tag-list-chips">${chipsList || '<span class="subtitle">None</span>'}</div>
            </td>
            <td>
                <button class="btn-trash-row" onclick="deleteCert(${cert.id})" title="Delete">
                    <i data-lucide="trash-2"></i>
                </button>
            </td>
        `;
        tbody.appendChild(row);
    });
}

function renderCoursesTable() {
    const tbody = document.getElementById("coursesTableBody");
    if (!tbody) return;
    tbody.innerHTML = "";

    if (appState.courses.length === 0) {
        tbody.innerHTML = `<tr><td colspan="6" class="timeline-empty-state">No courses registered.</td></tr>`;
        return;
    }

    appState.courses.forEach(course => {
        const progressHours = appState.progressEntries
            .filter(p => p.courseId === course.id)
            .reduce((sum, p) => sum + parseFloat(p.hours), 0.0);

        const row = document.createElement("tr");
        row.innerHTML = `
            <td class="cert-title-cell">${course.title}</td>
            <td><span class="provider-capsule">${course.provider}</span></td>
            <td><span class="level-pill-badge level-${course.status ? course.status.toLowerCase() : 'inprogress'}">${course.status || 'InProgress'}</span></td>
            <td>
                ${course.url ? `<a href="${course.url}" target="_blank" class="table-link-btn">Link <i data-lucide="arrow-up-right"></i></a>` : '<span class="subtitle">—</span>'}
            </td>
            <td class="font-mono">${progressHours.toFixed(1)} hrs</td>
            <td style="display: flex; gap: 0.5rem;">
                <button class="btn-primary-pill" style="padding: 0.4rem 0.75rem; min-height: unset; font-size: 0.75rem;" onclick="getAiTips('${encodeURIComponent(course.title)}')" title="Ask AI Coach for study tips">
                    <span>AI Tips</span>
                    <i data-lucide="sparkles" style="width: 12px; height: 12px;"></i>
                </button>
                <button class="btn-trash-row" onclick="deleteCourse(${course.id})" title="Delete">
                    <i data-lucide="trash-2"></i>
                </button>
            </td>
        `;
        tbody.appendChild(row);
    });
}

async function deleteCourse(id) {
    if (!confirm("Are you sure you want to delete this course?")) return;

    if (appState.isLive) {
        try {
            await authenticatedFetch(`${appState.apiUrl}/api/v1/courses/${id}`, { method: 'DELETE' });
            await loadDataFromApi();
            renderAll();
        } catch (e) {
            alert("Connection error.");
        }
    } else {
        appState.courses = appState.courses.filter(c => c.id !== id);
        renderAll();
    }
}

async function getAiTips(encodedTitle) {
    if (!appState.isLive) {
        showToast("AI Coach requires an active API connection.", "warning");
        return;
    }

    const title = decodeURIComponent(encodedTitle);
    const modal = document.getElementById('aiTipsModal');
    const content = document.getElementById('aiTipsContent');
    
    content.innerHTML = '<div style="display: flex; align-items: center; justify-content: center; height: 100px;"><div class="brand-square" style="animation: breathing 2s infinite ease-in-out;"><i data-lucide="sparkles"></i></div></div>';
    modal.classList.add('active');
    lucide.createIcons();

    try {
        const data = await authenticatedFetch(`${appState.apiUrl}/api/v1/coach/tips?title=${encodeURIComponent(title)}`);
        
        if (data && data.tips) {
            content.innerHTML = data.tips.replace(/\n/g, '<br>').replace(/\*\*(.*?)\*\*/g, '<strong>$1</strong>');
        } else {
            content.innerHTML = `<span style="color: var(--danger)">No tips returned.</span>`;
        }
    } catch (e) {
        content.innerHTML = `<span style="color: var(--danger)">Error: ${e.message || "Failed to reach the AI Coach service."}</span>`;
    }
}

// Dropdowns
function populateDropdowns() {
    const targetSelect = document.getElementById("progressTargetSelect");
    const skillSelect = document.getElementById("progressSkillSelect");

    if(targetSelect) targetSelect.innerHTML = `<option value="" disabled selected>Select credential or course target...</option>`;
    if(skillSelect) skillSelect.innerHTML = `<option value="">None - Log directly to target</option>`;

    if (targetSelect && appState.certifications.length > 0) {
        const certGroup = document.createElement("optgroup");
        certGroup.label = "Credentials";
        appState.certifications.forEach(cert => {
            const opt = document.createElement("option");
            opt.value = "cert_" + cert.id;
            opt.textContent = cert.title;
            certGroup.appendChild(opt);
        });
        targetSelect.appendChild(certGroup);
    }

    if (targetSelect && appState.courses.length > 0) {
        const courseGroup = document.createElement("optgroup");
        courseGroup.label = "Courses In Progress";
        appState.courses.forEach(course => {
            const opt = document.createElement("option");
            opt.value = "course_" + course.id;
            opt.textContent = course.title;
            courseGroup.appendChild(opt);
        });
        targetSelect.appendChild(courseGroup);
    }

    if (skillSelect) {
        appState.skills.forEach(skill => {
            const opt = document.createElement("option");
            opt.value = skill.id;
            opt.textContent = skill.name;
            skillSelect.appendChild(opt);
        });
    }
}

// Form Handlers
function setupForms() {
    // Create Skill Form
    document.getElementById("skillForm").addEventListener("submit", async (e) => {
        e.preventDefault();
        const request = {
            name: document.getElementById("skillName").value,
            description: document.getElementById("skillDesc").value,
            level: document.getElementById("skillLevel").value,
            targetHours: parseInt(document.getElementById("skillTargetHours").value)
        };

        if (appState.isLive) {
            try {
                await authenticatedFetch(`${appState.apiUrl}/api/v1/skills`, {
                    method: 'POST',
                    body: JSON.stringify(request)
                });
                await loadDataFromApi();
                closeModal("skillModal");
                document.getElementById("skillForm").reset();
                renderAll();
            } catch (error) {
                alert(`Error: ${error.message || 'Unable to save skill.'}`);
            }
        } else {
            // Mock Update
            const newSkill = {
                id: Date.now(),
                ...request,
                certificationCount: 0,
                createdAt: new Date(),
                updatedAt: new Date()
            };
            appState.skills.push(newSkill);
            closeModal("skillModal");
            document.getElementById("skillForm").reset();
            renderAll();
        }
    });

    // Create Credential Form
    document.getElementById("certForm").addEventListener("submit", async (e) => {
        e.preventDefault();
        const request = {
            title: document.getElementById("certTitle").value,
            provider: document.getElementById("certProvider").value,
            completedDate: new Date(document.getElementById("certDate").value).toISOString(),
            credentialUrl: document.getElementById("certUrl").value || null,
            skillIds: []
        };

        if (appState.isLive) {
            try {
                await authenticatedFetch(`${appState.apiUrl}/api/v1/certifications`, {
                    method: 'POST',
                    body: JSON.stringify(request)
                });
                await loadDataFromApi();
                closeModal("certModal");
                document.getElementById("certForm").reset();
                renderAll();
            } catch (error) {
                alert(`Error: ${error.message || 'Unable to register credential.'}`);
            }
        } else {
            const newCert = {
                id: Date.now(),
                ...request,
                createdAt: new Date(),
                updatedAt: new Date()
            };
            appState.certifications.push(newCert);
            closeModal("certModal");
            document.getElementById("certForm").reset();
            
            loadMockData(); // Recompute mocks summary
            renderAll();
        }
    });

    // Log Progress Form
    document.getElementById("progressForm").addEventListener("submit", async (e) => {
        e.preventDefault();
        const skillIdVal = document.getElementById("progressSkillSelect").value;
        const targetVal = document.getElementById("progressTargetSelect").value;
        
        const request = {
            skillId: skillIdVal ? parseInt(skillIdVal) : null,
            hours: parseFloat(document.getElementById("progressHours").value),
            notes: document.getElementById("progressNotes").value
        };

        if (targetVal.startsWith("cert_")) {
            request.certificationId = parseInt(targetVal.replace("cert_", ""));
        } else if (targetVal.startsWith("course_")) {
            request.courseId = parseInt(targetVal.replace("course_", ""));
        }

        if (appState.isLive) {
            try {
                await authenticatedFetch(`${appState.apiUrl}/api/v1/progress`, {
                    method: 'POST',
                    body: JSON.stringify(request)
                });
                alert("Session logged successfully!");
                await loadDataFromApi();
                document.getElementById("progressForm").reset();
                document.getElementById("hoursVal").textContent = "2.0 hrs";
                
                // Direct to Dashboard Tab
                document.querySelector('.menu-item[data-tab="dashboard"]').click();
                renderAll();
            } catch (error) {
                alert(`Error: ${error.message || 'Unable to log session.'}`);
            }
        } else {
            const newProg = {
                id: Date.now(),
                ...request,
                recordedAt: new Date().toISOString()
            };
            appState.progressEntries.unshift(newProg);
            document.getElementById("progressForm").reset();
            document.getElementById("hoursVal").textContent = "2.0 hrs";
            
            loadMockData();
            
            // Direct to Dashboard Tab
            document.querySelector('.menu-item[data-tab="dashboard"]').click();
            renderAll();
        }
    });
    // Create Course Form
    const courseForm = document.getElementById("courseForm");
    if(courseForm) {
        courseForm.addEventListener("submit", async (e) => {
            e.preventDefault();
            const request = {
                title: document.getElementById("courseTitle").value,
                provider: document.getElementById("courseProvider").value,
                url: document.getElementById("courseUrl").value || null
            };

            if (appState.isLive) {
                try {
                    await authenticatedFetch(`${appState.apiUrl}/api/v1/courses`, {
                        method: 'POST',
                        body: JSON.stringify(request)
                    });
                    await loadDataFromApi();
                    closeModal("courseModal");
                    document.getElementById("courseForm").reset();
                    renderAll();
                } catch (error) {
                    alert(`Error: ${error.message || 'Unable to register course.'}`);
                }
            } else {
                const newCourse = {
                    id: Date.now(),
                    ...request,
                    status: "InProgress",
                    createdAt: new Date()
                };
                appState.courses.push(newCourse);
                closeModal("courseModal");
                document.getElementById("courseForm").reset();
                renderAll();
            }
        });
    }
}

// Delete handlers
async function deleteSkill(id) {
    if (!confirm("Are you sure you want to delete this skill domain?")) return;

    if (appState.isLive) {
        try {
            await authenticatedFetch(`${appState.apiUrl}/api/v1/skills/${id}`, { method: 'DELETE' });
            await loadDataFromApi();
            renderAll();
        } catch (e) {
            alert(`Error: ${e.message || 'Connection error.'}`);
        }
    } else {
        appState.skills = appState.skills.filter(s => s.id !== id);
        renderAll();
    }
}

async function deleteCert(id) {
    if (!confirm("Are you sure you want to delete this credential permanent?")) return;

    if (appState.isLive) {
        try {
            await authenticatedFetch(`${appState.apiUrl}/api/v1/certifications/${id}`, { method: 'DELETE' });
            await loadDataFromApi();
            renderAll();
        } catch (e) {
            alert(`Error: ${e.message || 'Connection error.'}`);
        }
    } else {
        appState.certifications = appState.certifications.filter(c => c.id !== id);
        loadMockData();
        renderAll();
    }
}
