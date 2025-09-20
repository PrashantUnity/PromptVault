// Theme management
window.setTheme = (theme) => {
    const root = document.documentElement;
    if (theme === 'dark') {
        root.classList.add('dark');
    } else {
        root.classList.remove('dark');
    }
    localStorage.setItem('theme', theme);
};

window.getTheme = () => {
    return localStorage.getItem('theme') || 'light';
};

window.toggleTheme = () => {
    const currentTheme = window.getTheme();
    const newTheme = currentTheme === 'light' ? 'dark' : 'light';
    window.setTheme(newTheme);
    return newTheme;
};

// Initialize theme on page load
document.addEventListener('DOMContentLoaded', () => {
    const savedTheme = window.getTheme();
    window.setTheme(savedTheme);
});

// File download utility
window.downloadFile = (filename, content) => {
    const blob = new Blob([content], { type: 'application/json' });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = filename;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
};


// Toast notification system
window.showToast = (message, type = 'info') => {
    // Simple toast implementation - can be enhanced with a proper toast library
    const toast = document.createElement('div');
    toast.className = `fixed top-4 right-4 p-4 rounded-md shadow-lg z-50 ${
        type === 'success' ? 'bg-green-500 text-white' :
        type === 'error' ? 'bg-red-500 text-white' :
        type === 'warning' ? 'bg-yellow-500 text-white' :
        'bg-blue-500 text-white'
    }`;
    toast.textContent = message;
    
    document.body.appendChild(toast);
    
    setTimeout(() => {
        toast.remove();
    }, 3000);
};

// Copy to clipboard
window.copyToClipboard = async (text) => {
    try {
        await navigator.clipboard.writeText(text);
        window.showToast('Copied to clipboard!', 'success');
    } catch (err) {
        // Fallback for older browsers
        const textArea = document.createElement('textarea');
        textArea.value = text;
        document.body.appendChild(textArea);
        textArea.select();
        document.execCommand('copy');
        document.body.removeChild(textArea);
        window.showToast('Copied to clipboard!', 'success');
    }
};

// SVG icons are now inline - no initialization needed
console.log('Using inline SVG icons - no external icon library required');


// Close modal when clicking outside
document.addEventListener('click', (e) => {
    if (e.target.classList.contains('modal')) {
        e.target.style.display = 'none';
        document.body.style.overflow = 'auto';
    }
});

// Keyboard shortcuts
document.addEventListener('keydown', (e) => {
    // Escape key to close modals
    if (e.key === 'Escape') {
        const visibleModal = document.querySelector('.modal[style*="display: flex"]');
        if (visibleModal) {
            visibleModal.style.display = 'none';
            document.body.style.overflow = 'auto';
        }
    }
    
    // Ctrl/Cmd + K for search
    if ((e.ctrlKey || e.metaKey) && e.key === 'k') {
        e.preventDefault();
        const searchInput = document.querySelector('input[type="text"]');
        if (searchInput) {
            searchInput.focus();
        }
    }
});


// File input click helper
window.clickFileInput = (elementRef) => {
    elementRef.click();
};

// Screen size detection for responsive layout
window.getWindowWidth = () => {
    return window.innerWidth;
};

// Resize listener management for ResponsiveLayout component
window.addResizeListener = (dotNetObjectRef) => {
    const handleResize = () => {
        try {
            dotNetObjectRef.invokeMethodAsync('OnResize');
        } catch (error) {
            console.error('Error calling OnResize:', error);
        }
    };
    
    window.addEventListener('resize', handleResize);
    
    // Store the handler and reference for cleanup
    if (!window._resizeHandlers) {
        window._resizeHandlers = new Map();
    }
    window._resizeHandlers.set(dotNetObjectRef, handleResize);
};

window.removeResizeListener = (dotNetObjectRef) => {
    if (window._resizeHandlers && window._resizeHandlers.has(dotNetObjectRef)) {
        const handler = window._resizeHandlers.get(dotNetObjectRef);
        window.removeEventListener('resize', handler);
        window._resizeHandlers.delete(dotNetObjectRef);
    }
};
