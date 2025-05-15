// Simple script to handle project filtering and searching
document.addEventListener('DOMContentLoaded', function() {
    console.log('Explore page script loaded');
    
    // Get the input elements
    var searchInput = document.querySelector('.search-input');
    var searchButton = document.querySelector('.search-button');
    var categoryFilter = document.getElementById('category-filter');
    var budgetFilter = document.getElementById('budget-filter');
    var deadlineFilter = document.getElementById('deadline-filter');
    
    // Get all project cards
    var projectCards = document.querySelectorAll('.project-card');
    
    if (!projectCards.length) {
        console.error('No project cards found on the page');
        return;
    }
    
    console.log('Found ' + projectCards.length + ' project cards');
    
    // Function to filter projects based on search and filter values
    function filterProjects() {
        var searchValue = searchInput ? searchInput.value.toLowerCase() : '';
        var categoryValue = categoryFilter ? categoryFilter.value.toLowerCase() : '';
        var budgetValue = budgetFilter ? budgetFilter.value.toLowerCase() : '';
        var deadlineValue = deadlineFilter ? deadlineFilter.value.toLowerCase() : '';
        
        console.log('Filtering with:', { 
            search: searchValue, 
            category: categoryValue, 
            budget: budgetValue, 
            deadline: deadlineValue 
        });
        
        // Track visible cards count
        var visibleCount = 0;
        
        // Loop through each project card
        projectCards.forEach(function(card) {
            // Get text content to search in
            var title = card.querySelector('h3') ? card.querySelector('h3').textContent.toLowerCase() : '';
            var description = card.querySelector('.project-description') ? 
                              card.querySelector('.project-description').textContent.toLowerCase() : '';
            
            // Get skills from tag elements
            var skills = [];
            var skillTags = card.querySelectorAll('.skill-tag');
            skillTags.forEach(function(tag) {
                skills.push(tag.textContent.toLowerCase());
            });
            
            // Check if card matches search term
            var matchesSearch = !searchValue || 
                               title.indexOf(searchValue) !== -1 || 
                               description.indexOf(searchValue) !== -1 || 
                               skills.some(function(skill) { return skill.indexOf(searchValue) !== -1; });
            
            // Check if card matches category filter
            var matchesCategory = !categoryValue;
            if (categoryValue && card.closest('.project-category')) {
                var categoryHeading = card.closest('.project-category').querySelector('h2');
                if (categoryHeading) {
                    matchesCategory = categoryHeading.textContent.toLowerCase().indexOf(categoryValue) !== -1;
                }
            }
            
            // Check if card matches budget filter
            var matchesBudget = !budgetValue;
            var budgetElement = card.querySelector('.project-budget');
            if (budgetValue && budgetElement) {
                matchesBudget = budgetElement.textContent.toLowerCase().indexOf(budgetValue) !== -1;
            }
            
            // Check if card matches deadline filter
            var matchesDeadline = !deadlineValue;
            var deadlineElement = card.querySelector('.project-deadline');
            if (deadlineValue && deadlineElement) {
                matchesDeadline = deadlineElement.textContent.toLowerCase().indexOf(deadlineValue) !== -1;
            }
            
            // Show or hide card based on all filters
            var isVisible = matchesSearch && matchesCategory && matchesBudget && matchesDeadline;
            
            if (isVisible) {
                card.style.display = '';
                visibleCount++;
            } else {
                card.style.display = 'none';
            }
        });
        
        console.log('Visible projects after filtering:', visibleCount);
        
        // Show/hide category sections based on whether they have visible cards
        var categories = document.querySelectorAll('.project-category');
        categories.forEach(function(category) {
            var visibleCards = category.querySelectorAll('.project-card:not([style*="display: none"])');
            if (visibleCards.length) {
                category.style.display = '';
            } else {
                category.style.display = 'none';
            }
        });
        
        // Show "no results" message if needed
        var noResultsMessage = document.querySelector('.no-projects');
        if (visibleCount === 0) {
            if (!noResultsMessage) {
                noResultsMessage = document.createElement('div');
                noResultsMessage.className = 'no-projects';
                noResultsMessage.innerHTML = '<h3>No projects found</h3><p>Try adjusting your search criteria or filters.</p>';
                
                var projectsContent = document.querySelector('.projects-content');
                if (projectsContent) {
                    projectsContent.appendChild(noResultsMessage);
                }
            } else {
                noResultsMessage.style.display = '';
            }
        } else if (noResultsMessage) {
            noResultsMessage.style.display = 'none';
        }
    }
    
    // Add event listeners
    if (searchInput) {
        searchInput.addEventListener('input', filterProjects);
        searchInput.addEventListener('keypress', function(e) {
            if (e.key === 'Enter') {
                filterProjects();
                e.preventDefault();
            }
        });
    }
    
    if (searchButton) {
        searchButton.addEventListener('click', function(e) {
            filterProjects();
            e.preventDefault();
        });
    }
    
    if (categoryFilter) categoryFilter.addEventListener('change', filterProjects);
    if (budgetFilter) budgetFilter.addEventListener('change', filterProjects);
    if (deadlineFilter) deadlineFilter.addEventListener('change', filterProjects);
    
    // Run initial filtering
    filterProjects();
    
    console.log('Explore page script initialization complete');
}); 