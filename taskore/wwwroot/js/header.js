document.addEventListener('DOMContentLoaded', function() {
    console.log('Header.js loaded'); // Debug message
    
    // Find user menu element
    const userMenu = document.querySelector('.user-menu');
    
    if (userMenu) {
        console.log('User menu found'); // Debug message
        
        // Add 'show-dropdown' class to control dropdown visibility
        const handleUserMenuClick = function(e) {
            console.log('User menu clicked'); // Debug message
            
            // Don't handle clicks on dropdown items (they should work normally)
            if (e.target.closest('.dropdown-item')) {
                return;
            }
            
            // Stop event propagation to prevent it from reaching document click handler
            e.stopPropagation();
            
            // Toggle the class that controls dropdown visibility
            userMenu.classList.toggle('show-dropdown');
        };
        
        // Close dropdown when clicking anywhere else
        const handleDocumentClick = function(e) {
            if (!userMenu.contains(e.target)) {
                userMenu.classList.remove('show-dropdown');
            }
        };
        
        // Add event listeners
        userMenu.addEventListener('click', handleUserMenuClick);
        document.addEventListener('click', handleDocumentClick);
    } else {
        console.log('User menu not found'); // Debug message
    }
});
