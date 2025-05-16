document.addEventListener('DOMContentLoaded', function() {
    // Edit section functionality (for non-logged-in view)
    const editButtons = document.querySelectorAll('.edit-btn');
    const editModal = document.getElementById('edit-modal');
    const closeModalBtn = document.getElementById('close-modal');
    const cancelEditBtn = document.getElementById('cancel-edit');
    const saveEditBtn = document.getElementById('save-edit');
    const modalTitle = document.getElementById('modal-title');
    const modalBody = document.getElementById('modal-body');
    
    // Get the anti-forgery token for AJAX requests
    const tokenElement = document.querySelector('input[name="__RequestVerificationToken"]');
    const antiForgeryToken = tokenElement ? tokenElement.value : '';
    
    let currentSection = '';
    let originalContent = '';
    
    // Set up form submission handlers for logged-in users
    setupFormHandlers();
    
    // Handle modal editing for non-logged-in users
    editButtons.forEach(button => {
        button.addEventListener('click', function() {
            const section = this.getAttribute('data-section');
            currentSection = section;
            
            // Set modal title based on section
            switch(section) {
                case 'about':
                    modalTitle.textContent = 'Edit About Me';
                    originalContent = document.getElementById('about-content').innerHTML;
                    modalBody.innerHTML = `
                        <textarea id="edit-about" class="edit-textarea" rows="6">${document.getElementById('about-content').innerText.trim()}</textarea>
                    `;
                    break;
                case 'skills':
                    const userType = document.body.getAttribute('data-user-type') || 'freelancer';
                    modalTitle.textContent = (userType === 'freelancer') ? 'Edit My Skills' : 'Edit Skills I\'m Looking For';
                    originalContent = document.getElementById('skills-content').innerHTML;
                    
                    // Get existing skills as comma-separated string
                    const skillTags = document.querySelectorAll('.skill-tag');
                    const skillsText = Array.from(skillTags).map(tag => tag.textContent).join(', ');
                    
                    modalBody.innerHTML = `
                        <p>Enter your skills separated by commas</p>
                        <textarea id="edit-skills" class="edit-textarea" rows="4">${skillsText}</textarea>
                    `;
                    break;
                case 'preferences':
                    modalTitle.textContent = 'Edit My Preferences';
                    originalContent = document.getElementById('preferences-content').innerHTML;
                    
                    // Create form with existing preference values
                    const preferenceItems = document.querySelectorAll('.preference-item');
                    let preferencesForm = '<div class="edit-preferences">';
                    
                    preferenceItems.forEach(item => {
                        const label = item.querySelector('.preference-label').textContent.replace(':', '');
                        const value = item.querySelector('.preference-value').textContent;
                        
                        preferencesForm += `
                            <div class="preference-edit-item">
                                <label>${label}:</label>
                                <input type="text" value="${value}" class="preference-input" data-field="${convertToDbFieldName(label)}">
                            </div>
                        `;
                    });
                    
                    preferencesForm += '</div>';
                    modalBody.innerHTML = preferencesForm;
                    break;
                case 'contact':
                    modalTitle.textContent = 'Edit Contact Information';
                    const contactItems = document.querySelectorAll('.contact-item');
                    let contactForm = '<div class="edit-contact">';
                    
                    contactItems.forEach(item => {
                        const icon = item.querySelector('i').className;
                        const value = item.querySelector('span').textContent;
                        let label = '';
                        let fieldName = '';
                        
                        if (icon.includes('envelope')) {
                            label = 'Email';
                            fieldName = 'Email';
                        }
                        else if (icon.includes('phone')) {
                            label = 'Phone';
                            fieldName = 'Phone';
                        }
                        else if (icon.includes('map-marker')) {
                            label = 'Location';
                            fieldName = 'Location';
                        }
                        else if (icon.includes('globe')) {
                            label = 'Website';
                            fieldName = 'Website';
                        }
                        
                        contactForm += `
                            <div class="contact-edit-item">
                                <label>${label}:</label>
                                <input type="text" value="${value}" class="contact-input" data-field="${fieldName}">
                            </div>
                        `;
                    });
                    
                    contactForm += '</div>';
                    modalBody.innerHTML = contactForm;
                    break;
                case 'availability':
                    const userTypeForAvailability = document.body.getAttribute('data-user-type') || 'freelancer';
                    if (userTypeForAvailability === 'freelancer') {
                        modalTitle.textContent = 'Edit Availability';
                        
                        const status = document.querySelector('.availability-status').textContent.trim();
                        const hours = document.querySelector('.availability-hours span').textContent;
                        
                        modalBody.innerHTML = `
                            <div class="edit-availability">
                                <div class="availability-edit-item">
                                    <label>Status:</label>
                                    <select id="availability-status" data-field="AvailabilityStatus">
                                        <option value="available" ${status.includes('Available for work') ? 'selected' : ''}>Available for work</option>
                                        <option value="limited" ${status.includes('Limited availability') ? 'selected' : ''}>Limited availability</option>
                                        <option value="unavailable" ${status.includes('Not available') ? 'selected' : ''}>Not available</option>
                                    </select>
                                </div>
                                <div class="availability-edit-item">
                                    <label>Hours per week:</label>
                                    <input type="number" min="1" max="60" value="${hours.split(' ')[0]}" id="availability-hours" data-field="AvailabilityHours">
                                </div>
                            </div>
                        `;
                    } else {
                        modalTitle.textContent = 'Edit Budget Range';
                        
                        const budgetRange = document.querySelector('.budget-range .budget-value').textContent;
                        const paymentType = document.querySelector('.budget-preference .budget-value').textContent;
                        
                        modalBody.innerHTML = `
                            <div class="edit-budget">
                                <div class="budget-edit-item">
                                    <label>Typical Budget:</label>
                                    <input type="text" value="${budgetRange}" id="budget-range">
                                </div>
                                <div class="budget-edit-item">
                                    <label>Preferred Payment:</label>
                                    <select id="payment-type">
                                        <option value="fixed" ${paymentType.includes('Fixed') ? 'selected' : ''}>Fixed Price</option>
                                        <option value="hourly" ${paymentType.includes('Hourly') ? 'selected' : ''}>Hourly Rate</option>
                                        <option value="milestone" ${paymentType.includes('Milestone') ? 'selected' : ''}>Milestone Based</option>
                                    </select>
                                </div>
                            </div>
                        `;
                    }
                    break;
            }
            
            // Show the modal
            editModal.classList.add('show');
        });
    });
    
    // Close modal events
    closeModalBtn.addEventListener('click', closeModal);
    cancelEditBtn.addEventListener('click', closeModal);
    
    function closeModal() {
        editModal.classList.remove('show');
    }
    
    // Helper function to convert UI labels to database field names
    function convertToDbFieldName(label) {
        const fieldMap = {
            'Preferred Project Types': 'PreferredProjectTypes',
            'Hourly Rate': 'HourlyRate',
            'Project Duration': 'ProjectDuration',
            'Communication Style': 'CommunicationStyle',
            'Project Timeline': 'ProjectDuration',
            'Communication Frequency': 'CommunicationStyle',
            'Preferred Team Size': 'PreferredProjectTypes',
            'Freelancer Experience': 'PreferredProjectTypes',
            'Project Types': 'PreferredProjectTypes',
            'Budget Range': 'HourlyRate'
        };
        
        return fieldMap[label] || label;
    }
    
    // Save changes via modal (for non-logged-in view)
    saveEditBtn.addEventListener('click', function() {
        // Different save logic based on the section
        const userData = {};
        
        switch(currentSection) {
            case 'about':
                const aboutText = document.getElementById('edit-about').value;
                document.getElementById('about-content').innerHTML = `<p>${aboutText.replace(/\n/g, '</p><p>')}</p>`;
                userData.About = aboutText;
                break;
            case 'skills':
                const skillsText = document.getElementById('edit-skills').value;
                const skills = skillsText.split(',').map(skill => skill.trim()).filter(skill => skill);
                
                let skillsHtml = '<div class="skills-container">';
                skills.forEach(skill => {
                    skillsHtml += `<span class="skill-tag">${skill}</span>`;
                });
                skillsHtml += '</div>';
                
                document.getElementById('skills-content').innerHTML = skillsHtml;
                userData.Skills = skillsText;
                break;
            case 'preferences':
                const preferenceInputs = document.querySelectorAll('.preference-input');
                const preferenceItems = document.querySelectorAll('.preference-item');
                
                preferenceInputs.forEach((input, index) => {
                    if (preferenceItems[index]) {
                        const value = input.value;
                        preferenceItems[index].querySelector('.preference-value').textContent = value;
                        
                        // Add to userData object with proper field name
                        const fieldName = input.getAttribute('data-field');
                        if (fieldName) {
                            userData[fieldName] = value;
                        }
                    }
                });
                break;
            case 'contact':
                const contactInputs = document.querySelectorAll('.contact-input');
                const contactItems = document.querySelectorAll('.contact-item span');
                
                contactInputs.forEach((input, index) => {
                    if (contactItems[index]) {
                        const value = input.value;
                        contactItems[index].textContent = value;
                        
                        // Add to userData object with proper field name
                        const fieldName = input.getAttribute('data-field');
                        if (fieldName) {
                            userData[fieldName] = value;
                        }
                    }
                });
                break;
            case 'availability':
                const userType = document.body.getAttribute('data-user-type') || 'freelancer';
                if (userType === 'freelancer') {
                    const status = document.getElementById('availability-status').value;
                    const hours = document.getElementById('availability-hours').value;
                    
                    const statusElement = document.querySelector('.availability-status');
                    const hoursElement = document.querySelector('.availability-hours span');
                    
                    // Update status text and class
                    statusElement.className = 'availability-status ' + status;
                    
                    if (status === 'available') {
                        statusElement.textContent = 'Available for work';
                    } else if (status === 'limited') {
                        statusElement.textContent = 'Limited availability';
                    } else {
                        statusElement.textContent = 'Not available';
                    }
                    
                    hoursElement.textContent = `${hours} hrs/week`;
                    
                    // Add to userData object
                    userData.AvailabilityStatus = status;
                    userData.AvailabilityHours = `${hours} hrs/week`;
                } else {
                    const budgetRange = document.getElementById('budget-range').value;
                    const paymentType = document.getElementById('payment-type').value;
                    
                    const rangeElement = document.querySelector('.budget-range .budget-value');
                    const typeElement = document.querySelector('.budget-preference .budget-value');
                    
                    rangeElement.textContent = budgetRange;
                    
                    if (paymentType === 'fixed') {
                        typeElement.textContent = 'Fixed Price';
                    } else if (paymentType === 'hourly') {
                        typeElement.textContent = 'Hourly Rate';
                    } else {
                        typeElement.textContent = 'Milestone Based';
                    }
                }
                break;
        }
        
        // Save to server via AJAX
        saveToServer(userData);
        
        // Close the modal
        closeModal();
    });
    
    // Set up form handlers for logged-in view
    function setupFormHandlers() {
        const forms = [
            document.getElementById('headline-form'),
            document.getElementById('about-form'),
            document.getElementById('skills-form'),
            document.getElementById('preferences-form'),
            document.getElementById('contact-form'),
            document.getElementById('availability-form')
        ];
        
        forms.forEach(form => {
            if (form) {
                form.addEventListener('submit', function(e) {
                    e.preventDefault();
                    
                    // Show a loading indicator
                    const saveButton = form.querySelector('.save-form-btn');
                    if (saveButton) {
                        const originalText = saveButton.textContent;
                        saveButton.textContent = 'Saving...';
                        saveButton.disabled = true;
                        
                        // Set a timeout to restore the button if the AJAX request fails
                        setTimeout(() => {
                            if (saveButton.textContent === 'Saving...') {
                                saveButton.textContent = originalText;
                                saveButton.disabled = false;
                            }
                        }, 10000); // 10 seconds timeout
                    }
                    
                    const formData = new FormData(form);
                    const userData = {};
                    
                    // Convert FormData to userData object
                    for (const [key, value] of formData.entries()) {
                        userData[key] = value;
                    }
                    
                    // Special handling for skills to format them properly
                    if (userData.Skills) {
                        const skillsText = userData.Skills;
                        const skills = skillsText.split(',').map(skill => skill.trim()).filter(skill => skill);
                        
                        // Reformat skills for database
                        userData.Skills = skills.join(', ');
                    }
                    
                    // Special handling for availability
                    if (userData.AvailabilityHours) {
                        userData.AvailabilityHours = `${userData.AvailabilityHours} hrs/week`;
                    }
                    
                    // Save the data to the server
                    saveToServer(userData, function(success) {
                        // Restore the button text
                        if (saveButton) {
                            saveButton.textContent = originalText;
                            saveButton.disabled = false;
                        }
                        
                        // Update UI if successful
                        if (success && form.id === 'skills-form' && userData.Skills) {
                            // Update the skills tags visually
                            const skills = userData.Skills.split(',').map(skill => skill.trim()).filter(skill => skill);
                            
                            let skillsHtml = '<div class="skills-container">';
                            skills.forEach(skill => {
                                skillsHtml += `<span class="skill-tag">${skill}</span>`;
                            });
                            skillsHtml += '</div>';
                            
                            // Add updated skills HTML to the skills form
                            const skillsDisplay = document.createElement('div');
                            skillsDisplay.classList.add('skills-preview');
                            skillsDisplay.innerHTML = skillsHtml;
                            
                            // Remove any existing preview
                            const existingPreview = form.querySelector('.skills-preview');
                            if (existingPreview) {
                                form.removeChild(existingPreview);
                            }
                            
                            // Insert new preview after the form inputs
                            form.insertBefore(skillsDisplay, form.querySelector('button'));
                        }
                    });
                });
            }
        });
    }
    
    function saveToServer(userData, callback) {
        // Check if we have data to save
        if (Object.keys(userData).length === 0) {
            if (callback) callback(false);
            return;
        }
        
        // Add the anti-forgery token
        userData.__RequestVerificationToken = antiForgeryToken;
        
        // Log what we're sending for debugging
        console.log('Saving user data:', userData);
        
        // Create XMLHttpRequest
        const xhr = new XMLHttpRequest();
        xhr.open('POST', '/Main/UpdateUserProfile', true);
        xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
        
        // Convert data to URL-encoded form data
        const formData = new URLSearchParams();
        for (const key in userData) {
            formData.append(key, userData[key]);
        }
        
        xhr.onreadystatechange = function() {
            if (xhr.readyState === 4) {
                try {
                    // Check if response is JSON
                    if (xhr.getResponseHeader('Content-Type') && xhr.getResponseHeader('Content-Type').indexOf('application/json') !== -1) {
                        const response = JSON.parse(xhr.responseText);
                        if (response.success) {
                            showNotification(response.message || 'Changes saved successfully!', 'success');
                            if (callback) callback(true);
                            
                            // Reload the page after a short delay to show the latest data
                            setTimeout(function() {
                                window.location.reload();
                            }, 1500);
                        } else {
                            showNotification(response.message || 'Error saving changes', 'error');
                            if (callback) callback(false);
                        }
                    } else {
                        // Handle HTML response (full page redirect)
                        showNotification('Changes saved successfully!', 'success');
                        if (callback) callback(true);
                        
                        // Reload the page after a short delay to show the latest data
                        setTimeout(function() {
                            window.location.reload();
                        }, 1500);
                    }
                } catch (e) {
                    console.error('Error parsing server response:', e);
                    // If response isn't JSON, it's probably an HTML page
                    if (xhr.status >= 200 && xhr.status < 300) {
                        showNotification('Changes saved successfully, reloading page...', 'success');
                        if (callback) callback(true);
                        
                        setTimeout(function() {
                            window.location.reload();
                        }, 1500);
                    } else {
                        showNotification('Error saving changes to server', 'error');
                        if (callback) callback(false);
                    }
                }
            }
        };
        
        xhr.onerror = function() {
            showNotification('Network error while saving changes', 'error');
            if (callback) callback(false);
        };
        
        // Send the request
        xhr.send(formData.toString());
    }
    
    // Notification function
    function showNotification(message, type) {
        const notification = document.createElement('div');
        notification.className = `notification ${type}`;
        notification.textContent = message;
        
        document.body.appendChild(notification);
        
        setTimeout(() => {
            notification.classList.add('show');
            
            setTimeout(() => {
                notification.classList.remove('show');
                setTimeout(() => {
                    notification.remove();
                }, 300);
            }, 3000);
        }, 10);
    }
}); 