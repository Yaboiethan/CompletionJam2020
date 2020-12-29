extends RigidBody

export var startingWeapon : PackedScene;
export var canMove: bool = true;

export var mouse_sensitivity : Vector2 = Vector2(1, 1) 
export(float, 0.0, 1.0) var mouse_acceleration : float = 0.5 

export var max_speed = 5.0
export var sprint_factor = 2.0
export(float, 0.0, 1.0) var acceleration_factor = 0.2
export var leg_default_length : float = 0.8

export var jump_speed = 8.0;

var maxHealth: int = 100;
var curHealth: int;


export var forward_action = "player_forwards"
export var backwards_action = "player_backwards"
export var left_action = "player_left"
export var right_action = "player_right"
export var sprint_action = "player_sprint"
export var jump_action = "player_jump"

export var mouse_start_captured : bool = true

export var default_fov = 70.0
export var sprint_fov = 90.0
export(float, 0.0, 1.0) var fov_acceleration = 0.1
var target_fov = default_fov

var gunHolder : Spatial
var audio : AudioStreamPlayer

var target_velocity : Vector3 = Vector3.ZERO
var yaw_pitch : Vector2 = Vector2.ZERO
var target_yaw_pitch : Vector2 = Vector2.ZERO

var on_floor : bool = false
var floor_distance : float = 0.0
var is_jumping : bool = false

func _ready():
	$Camera.fov = default_fov
	if self.mouse_start_captured:
		Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED)
	gunHolder = get_node("Camera/Gun_Holder");
	audio = get_node("AudioStreamPlayer");
	# Equip starting gun
	if startingWeapon != null:
		var g = startingWeapon.instance();
		g.set("inUse", true);
		pickUpGun(g);
	curHealth = maxHealth;
	pass # Replace with function body.

func handle_orientation(event : InputEventMouseMotion):
	self.target_yaw_pitch -= event.relative*self.mouse_sensitivity*PI/180.0;
	self.target_yaw_pitch.x = wrapf(self.target_yaw_pitch.x, -PI, PI);
	self.target_yaw_pitch.y = clamp(self.target_yaw_pitch.y, -PI/2.0, PI/2.0);

func handle_focus(event : InputEvent):
	if event is InputEventMouseButton and Input.get_mouse_mode() == Input.MOUSE_MODE_VISIBLE:
		Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED);
	elif event.is_action_pressed(self.uncapture_action) and Input.get_mouse_mode() == Input.MOUSE_MODE_CAPTURED:
		Input.set_mouse_mode(Input.MOUSE_MODE_VISIBLE);

func _unhandled_input(event : InputEvent):
	#self.handle_focus(event);
	if Input.get_mouse_mode() == Input.MOUSE_MODE_CAPTURED:
		if event is InputEventMouseMotion:
			handle_orientation(event)
			
	if event.is_action_pressed("Player_Switch_Knife"):
		if gunHolder.get_child_count() > 1:
			var knife: Node = gunHolder.get_child(0);
			var gun: Node = gunHolder.get_child(1);
			gun.visible = false;
			gun.set("inUse", false);
			knife.visible = true;
			knife.set("inUse", true);
			knife.get("uiManager").call("updateAmmoUI");
	elif event.is_action_pressed("Player_Switch_Gun"):
		if gunHolder.get_child_count() > 1:
			var knife: Node = gunHolder.get_child(0);
			var gun: Node = gunHolder.get_child(1);
			gun.visible = true;
			gun.set("inUse", true);
			knife.visible = false;
			knife.set("inUse", false);
			gun.call("updateUI");
		pass

func handle_input():
	self.target_velocity = Vector3.ZERO;
	if self.on_floor and Input.is_action_pressed(jump_action):
		self.target_velocity.y = self.jump_speed;
		self.is_jumping = true;
		self.on_floor = false;
	else:
		self.target_velocity.y = self.linear_velocity.y;
		self.is_jumping = false;
	
	var planar_velocity : Vector2 = Vector2.ZERO;
	planar_velocity.y = (Input.get_action_strength(forward_action) - Input.get_action_strength(backwards_action))
	planar_velocity.x = (Input.get_action_strength(right_action) - Input.get_action_strength(left_action));
	if planar_velocity.length_squared() > 1.0:
		planar_velocity = planar_velocity.normalized()
	planar_velocity *= self.max_speed*(1.0 + Input.get_action_strength(sprint_action)*(self.sprint_factor - 1))
	planar_velocity = planar_velocity.rotated(self.yaw_pitch.x)
	if Input.is_action_pressed(sprint_action) and planar_velocity != Vector2.ZERO:
		self.target_fov = self.sprint_fov
	else:
		self.target_fov = self.default_fov
	
	self.target_velocity += Vector3.RIGHT*planar_velocity.x + Vector3.FORWARD*planar_velocity.y;

func handle_movement(delta):
	#up-down movement
	if self.on_floor:
		var displacement_correction = self.mass*(leg_default_length - self.floor_distance)/delta
		var weight_correction = self.weight*delta
		var speed_correction = -self.mass*self.linear_velocity.y;
		var total_correction = displacement_correction*0.2 + weight_correction + speed_correction;
		self.apply_central_impulse(Vector3.UP*total_correction);
	#planar movement
	var velocity_correction = self.mass*(self.target_velocity - self.linear_velocity)
	velocity_correction.x *= acceleration_factor;
	velocity_correction.z *= acceleration_factor;
	self.apply_central_impulse(velocity_correction);

func handle_raycast():
	$RayCast.force_raycast_update();
	if $RayCast.is_colliding():
		self.on_floor = true;
		var point = $RayCast.get_collision_point();
		self.floor_distance = $RayCast.to_local(point).length();
	else:
		self.on_floor = false;

func _process(_delta : float):
	if !canMove:
		return;
	self.yaw_pitch.x = lerp_angle(self.yaw_pitch.x, self.target_yaw_pitch.x, self.mouse_acceleration);
	self.yaw_pitch.y = lerp_angle(self.yaw_pitch.y, self.target_yaw_pitch.y, self.mouse_acceleration);
	$Camera.rotation = Vector3.ZERO;
	$Camera.rotate_x(self.yaw_pitch.y)
	$Camera.rotate_y(self.yaw_pitch.x)
	
	$Camera.fov = lerp($Camera.fov, self.target_fov, self.fov_acceleration)
	

func _physics_process(delta):
	if !canMove:
		return;
	handle_raycast();
	handle_input();
	handle_movement(delta);
	pass

func pickUpGun(Gun):
	var gunPickup = Gun.get_parent();
	#Check if player has gun to put back
	if gunHolder.get_child_count() > 0:
			gunHolder.get_child(0).visible = false;
			gunHolder.get_child(0).set("inUse", false);
	if gunHolder.get_child_count() > 1:
		gunPickup.call_deferred("gunSwap", gunHolder.get_child(1));
	if Gun.get_parent() != null:
		Gun.get_parent().remove_child(Gun);
	gunHolder.add_child(Gun);
	Gun.call("setAnimator", gunHolder);
	pass
	
func pickUpCollected(type, amt) -> bool:
	if type == "Ammo":
		#Check if at max
		var curA: int = gunHolder.get_child(1).get("curAmmo");
		var maxA: int = gunHolder.get_child(1).get("ammoMax")
		if curA >= maxA:
			return false;
		gunHolder.get_child(1).call("addBullets", amt);
		curA = gunHolder.get_child(1).get("curAmmo");
		#UI
		if gunHolder.get_child(1).get("inUse"):
			gunHolder.get_child(1).call("updateUI");
	elif type == "Health":
		#check if at max
		if curHealth >= maxHealth:
			return false;
		#Check if will reach max
		if curHealth + amt > maxHealth:
			curHealth = maxHealth;
		else:
			curHealth += amt;
		#UI
		get_tree().root.get_child(0).get_node("UI").call("updatePlayerHealthUI", curHealth);
	return true
	
func _on_Area_body_entered(body):
	if body.name.find("Bullet") != -1 && curHealth > 0:
		var damage : int = body.get("playerDamage");
		if curHealth - damage <= 0:
			curHealth = 0;
			canMove = false;
		else:
			curHealth -= damage;
			audio.play();
		#UI
		get_tree().root.get_child(0).get_node("UI").call("updatePlayerHealthUI", curHealth);
	pass # Replace with function body.
